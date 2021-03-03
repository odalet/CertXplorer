using System;
using Delta.CertXplorer.Extensibility;

namespace CertToolsPlugin
{
    internal class CertMmcPluginInfo : IPluginInfo
    {
        private static readonly Guid guid = new Guid("{6588DED6-558C-45A3-AF2D-7FDBB612641E}");

        public Guid Id => guid;
        public string Name => "Certificates MMC Plugin";
        public string Description => "This plugin launches a MMC console preconfigured with user and local machines certificates snap-ins";
        public string Author => "O. DALET";
        public string Company => ThisAssembly.Company;
        public string Version => ThisAssembly.PluginVersion;
    }
}
