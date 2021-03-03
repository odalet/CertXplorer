using System;
using Delta.CertXplorer.Extensibility;

namespace CryptoHelperPlugin.DataHandlers
{
    internal sealed class Base64HandlerPlugin : BaseDataHandlerPlugin
    {
        private static readonly IPluginInfo pluginInfo = new Base64HandlerPluginInfo();

        public override IPluginInfo PluginInfo => pluginInfo;
        protected override Guid PluginId => pluginInfo.Id;
        protected override string PluginName => pluginInfo.Name;
        public override IDataHandler CreateHandler() => new BinaryAsTextHandler(this, DataFormat.Base64);
    }
}
