using System;
using System.Text;

namespace Delta.CertXplorer.Logging
{
    /// <summary>
    /// This class stores the properties, a logging message is made of.
    /// </summary>
    public class LogEntry
    {
        public LogEntry() => TimeStamp = DateTime.Now;

        public LogLevel Level { get; set; }

        public LogSource Source { get; set; }

        public string SourceName
        {
            get => Source == null ? string.Empty : Source.Name;
            set => Source = new LogSource(value);
        }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public DateTime TimeStamp { get; set; }

        public object Tag { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            if (Source != null)
            {
                var sourceName = Source.ToString();
                if (!string.IsNullOrEmpty(sourceName))
                    _ = builder.Append($"[{sourceName}] ");
            }

            _ = builder.Append(Level.ToString().ToUpperInvariant());

            if (TimeStamp != DateTime.MinValue && TimeStamp != DateTime.MaxValue)
                _ = builder.Append($" [{TimeStamp:yyyy/MM/dd HH:mm:ss}]");

            _ = builder.Append(": ");

            var contentBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Message))
            {
                _ = contentBuilder.Append(Message);
                if (Exception != null) _ = contentBuilder.Append(" - ");
            }

            if (Exception != null)
                _ = contentBuilder.Append(Exception.ToFormattedString());

            var text = contentBuilder.ToString();
            _ = builder.Append(string.IsNullOrEmpty(text) ? "No message" : text);

            return builder.ToString();
        }
    }
}
