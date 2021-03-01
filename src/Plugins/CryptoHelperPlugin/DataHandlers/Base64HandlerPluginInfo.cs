using System;
using Delta.CertXplorer.Extensibility;

namespace CryptoHelperPlugin.DataHandlers
{
    internal sealed class Base64HandlerPluginInfo : IPluginInfo
    {
        private static readonly Guid guid = new Guid("{F017208B-06FC-4A4B-AC8E-387A9F06B7AE}");

        public Guid Id => guid;
        public string Name => "Base64 Plugin";
        public string Description => ThisAssembly.Description;
        public string Author => "O. DALET";
        public string Company => ThisAssembly.Company;
        public string Version => ThisAssembly.PluginVersion;
    }
}
