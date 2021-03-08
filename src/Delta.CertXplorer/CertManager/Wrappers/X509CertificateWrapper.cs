using System;
using System.Security.Cryptography.X509Certificates;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    internal sealed class X509CertificateWrapper : BaseWrapper
    {
        private readonly X509Certificate x509;

        public X509CertificateWrapper(X509Certificate certificate) => x509 = certificate;

        public IntPtr Handle => TryGet(() => x509.Handle);
        public string Issuer => TryGet(() => x509.Issuer);
        public string Subject => TryGet(() => x509.Subject);
    }
}
