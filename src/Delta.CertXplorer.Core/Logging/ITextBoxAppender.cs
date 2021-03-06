using System;

using Delta.CertXplorer.UI;

namespace Delta.CertXplorer.Logging
{
    public interface ITextBoxAppender : IDisposable
    {
        LogLevel LogThreshold { get; set; }
        ThreadSafeTextBoxWrapper TextBoxWrapper { get; }
    }
}
