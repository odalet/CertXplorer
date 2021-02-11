using System;
using Delta.CapiNet;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    internal sealed class CapiCtlWrapper : BaseWrapper
    {
        private readonly CertificateTrustList ctl;

        public CapiCtlWrapper(CertificateTrustList certificateTrustList) => ctl = certificateTrustList;

        public string FriendlyName => TryGet(() => ctl.FriendlyName);
        public DateTime PublicationDate => TryGet(() => ctl.PublicationDate);
        public DateTime NextUpdate => TryGet(() => ctl.NextUpdate);
        public bool IsValid => TryGet(() => ctl.IsValid);
    }
}
