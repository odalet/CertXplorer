using System;
using Delta.CapiNet;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    internal sealed class CapiCtlWrapper : BaseWrapper
    {
        private readonly CertificateTrustList ctl;

        public CapiCtlWrapper(CertificateTrustList certificateTrustList) => ctl = certificateTrustList;

        public string FriendlyName => TryGet(() => ctl.FriendlyName);
        public DateTimeOffset PublicationDate => TryGet(() => ctl.PublicationDate);
        public DateTimeOffset NextUpdate => TryGet(() => ctl.NextUpdate);
        public bool IsValid => TryGet(() => ctl.IsValid);
    }
}
