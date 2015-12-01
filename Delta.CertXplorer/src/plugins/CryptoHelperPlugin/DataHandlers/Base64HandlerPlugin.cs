using System;
using Delta.CertXplorer.Extensibility;

namespace CryptoHelperPlugin.DataHandlers
{
    internal class Base64HandlerPlugin : BaseDataHandlerPlugin
    {
        private static readonly IPluginInfo pluginInfo = new Base64HandlerPluginInfo();
        
        public override IPluginInfo PluginInfo
        {
            get { return pluginInfo; }
        }

        protected override Guid PluginId
        {
            get { return pluginInfo.Id; }
        }

        protected override string PluginName
        {
            get { return pluginInfo.Name; }
        }

        public override IDataHandler CreateHandler()
        {
            return new BinaryAsTextHandler(this, DataFormat.Base64);
        }
    }
}
