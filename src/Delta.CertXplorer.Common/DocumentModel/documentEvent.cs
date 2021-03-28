using System;

namespace Delta.CertXplorer.DocumentModel
{
    public delegate void DocumentEventHandler(object sender, DocumentEventArgs e);

    public sealed class DocumentEventArgs : EventArgs
    {
        public DocumentEventArgs(IDocument document) => Document = document;
        public IDocument Document { get; }
    }
}
