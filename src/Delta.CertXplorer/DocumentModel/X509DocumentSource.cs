using System;

namespace Delta.CertXplorer.DocumentModel
{
    internal sealed class X509DocumentSource : IDocumentSource
    {
        public X509DocumentSource(X509Object x509) =>
            X509Data = x509 ?? throw new ArgumentNullException(nameof(x509));

        public X509Object X509Data { get; }
        public string Uri => X509Data.DisplayName;
        public bool IsReadOnly => true;
    }
}
