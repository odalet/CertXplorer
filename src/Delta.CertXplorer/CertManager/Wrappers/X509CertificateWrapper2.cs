using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    internal class X509CertificateWrapper2 : X509CertificateWrapper
    {
        private readonly X509Certificate2 x509;
        private X509ExtensionWrapper[] extensions = null;

        public X509CertificateWrapper2(X509Certificate2 certificate) : base(certificate)
        {
            x509 = certificate;
            FillExtensions();
        }

        public bool Archived => x509.Archived;
        public X509ExtensionWrapper[] Extensions => TryGet(() => extensions);
        public string FriendlyName => TryGet(() => x509.FriendlyName);
        public bool HasPrivateKey => TryGet(() => x509.HasPrivateKey);
        public string IssuerName => TryGet(() => x509.IssuerName.Name);
        public DateTime NotAfter => TryGet(() => x509.NotAfter);
        public DateTime NotBefore => TryGet(() => x509.NotBefore);
        public AsymmetricAlgorithm PrivateKey => TryGet(() => x509.PrivateKey);
        public PublicKey PublicKey => TryGet(() => x509.PublicKey);
        public byte[] RawData => TryGet(() => x509.RawData);
        public string SerialNumber => TryGet(() => x509.SerialNumber);
        public Oid SignatureAlgorithm => TryGet(() => x509.SignatureAlgorithm);
        public string SubjectName => TryGet(() => x509.SubjectName.Name);
        public string Thumbprint => TryGet(() => x509.Thumbprint);
        public int Version => TryGet(() => x509.Version);

        private void FillExtensions() => extensions = x509.Extensions != null && x509.Extensions.Count > 0 ?
            x509.Extensions.Cast<X509Extension>().Select(x => X509ExtensionWrapper.Create(x)).ToArray() :
            new X509ExtensionWrapper[0];
    }
}
