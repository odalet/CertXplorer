using System;
using System.Text;

namespace Delta.CertXplorer.Extensibility.Logging
{
    public sealed class LogEntry
    {
        public LogEntry() => TimeStamp = DateTime.Now;
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public DateTime TimeStamp { get; set; }
        public object Tag { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder()
                .Append(Level.ToString().ToUpperInvariant())
                ;

            if (TimeStamp != DateTime.MinValue && TimeStamp != DateTime.MaxValue)
                _ = builder.Append($" [{TimeStamp:yyyy/MM/dd HH:mm:ss}]");

            _ = builder.Append(": ");

            var sbText = new StringBuilder();

            if (!string.IsNullOrEmpty(Message))
            {
                _ = sbText.Append(Message);
                if (Exception != null) 
                    _ = sbText.Append(" - ");
            }

            if (Exception != null)
                _ = sbText.Append(Exception.ToFormattedString());

            var text = sbText.ToString();
            _ = builder.Append(string.IsNullOrEmpty(text) ? "No message" : text);

            return builder.ToString();
        }
    }
}
