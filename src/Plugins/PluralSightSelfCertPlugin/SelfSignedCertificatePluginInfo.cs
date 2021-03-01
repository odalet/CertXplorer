using System;
using Delta.CertXplorer.Extensibility;

namespace PluralSightSelfCertPlugin
{
    internal sealed class SelfSignedCertificatePluginInfo : IPluginInfo
    {
        private static readonly Guid guid = new Guid("{1ABE44BB-39B2-4e99-937F-12C346B1B7C6}");

        public Guid Id => guid;
        public string Name => "Pluralsight's SelfCert";
        public string Description => ThisAssembly.Description;
        public string Author => "O. DALET based on Pluralsight's work";
        public string Company => ThisAssembly.Company;
        public string Version => ThisAssembly.PluginVersion;
    }
}
