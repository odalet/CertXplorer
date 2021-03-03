using System;
using Delta.CertXplorer.Extensibility;

namespace CryptoHelperPlugin
{
    internal sealed class PluginInfo : IPluginInfo
    {
        private static readonly Guid guid = new Guid("{19A43451-B110-4f9a-A103-63A2B569CA0C}");

        public Guid Id => guid;
        public string Name => "Crypto Helper";
        public string Description => ThisAssembly.Description;
        public string Author => "O. DALET";
        public string Company => ThisAssembly.Company;
        public string Version => ThisAssembly.PluginVersion;
    }
}
