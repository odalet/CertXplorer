using System;
using System.IO;
using Delta.CapiNet.Pem;
using Delta.CertXplorer.Extensibility;

namespace PemPlugin
{
    internal class PemHandlerPlugin : BaseDataHandlerPlugin
    {
        private sealed class PemHandler : IDataHandler
        {
            private readonly PemHandlerPlugin plugin;
            private byte[] fileContent = null;

            public PemHandler(PemHandlerPlugin parent) => plugin = parent ?? throw new ArgumentNullException("parent");

            public bool CanHandleFile(string filename)
            {
                if (!File.Exists(filename)) throw new FileNotFoundException(
                    $"File {filename} could not be found", filename);

                fileContent = File.ReadAllBytes(filename);

                return PemDecoder.IsPemData(fileContent);
            }

            public IData ProcessFile()
            {
                if (fileContent == null) throw new InvalidOperationException("Invalid input data: null");

                var decoder = new PemDecoder();
                var result = decoder.ReadData(fileContent);
                if (result == null)
                {
                    foreach (var error in decoder.Errors)
                        plugin.Log.Error(error);
                    return null;
                }

                foreach (var warning in result.Warnings)
                    plugin.Log.Warning(warning);

                return new PemData(result);
            }
        }

        private static readonly IPluginInfo pluginInfo = new PemHandlerPluginInfo();

        public override IPluginInfo PluginInfo => pluginInfo;
        protected override Guid PluginId => pluginInfo.Id;
        protected override string PluginName => pluginInfo.Name;

        public override IDataHandler CreateHandler() => new PemHandler(this);
    }
}
