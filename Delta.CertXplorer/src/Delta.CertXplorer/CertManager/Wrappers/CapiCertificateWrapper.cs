using Delta.CapiNet;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    internal class CapiCertificateWrapper : X509CertificateWrapper2
    {
        private readonly Certificate cert;

        public CapiCertificateWrapper(Certificate certificate) : base(certificate.X509) => cert = certificate;

        public bool IsValid => TryGet(() => cert.IsValid);
    }
}
