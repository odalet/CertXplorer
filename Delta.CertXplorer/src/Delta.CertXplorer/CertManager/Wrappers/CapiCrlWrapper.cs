using System;
using Delta.CapiNet;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    internal sealed class CapiCrlWrapper : BaseWrapper
    {
        private readonly CertificateRevocationList crl;

        public CapiCrlWrapper(CertificateRevocationList certificateRevocationList) => crl = certificateRevocationList;

        public string FriendlyName => TryGet(() => crl.FriendlyName);
        public string IssuerName => TryGet(() => crl.IssuerName.Name);
        public DateTime PublicationDate => TryGet(() => crl.PublicationDate);
        public DateTime NextUpdate => TryGet(() => crl.NextUpdate);
        public bool IsValid => TryGet(() => crl.IsValid);
    }
}
