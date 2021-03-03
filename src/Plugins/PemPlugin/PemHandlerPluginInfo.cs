using System;
using Delta.CertXplorer.Extensibility;

namespace PemPlugin
{
    internal class PemHandlerPluginInfo : IPluginInfo
    {
        private static readonly Guid guid = new Guid("{962F5C9E-E00C-467A-899A-E61BE4093258}");

        public Guid Id => guid;
        public string Name => "PEM Plugin";
        public string Description => ThisAssembly.Description;
        public string Author => "O. DALET";
        public string Company => ThisAssembly.Company;
        public string Version => ThisAssembly.PluginVersion;
    }
}
