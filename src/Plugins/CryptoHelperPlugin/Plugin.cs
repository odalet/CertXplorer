using System;
using System.Drawing;
using System.Windows.Forms;
using Delta.CertXplorer.Extensibility;
using Delta.CertXplorer.Extensibility.Logging;
using Delta.CertXplorer.Extensibility.UI;

namespace CryptoHelperPlugin
{
    internal sealed class Plugin : BaseGlobalPlugin
    {
        private static readonly PluginInfo pluginInfo = new PluginInfo();

        public override bool Run(IWin32Window owner)
        {
            try
            {
                LogService = Log;

                Log.Verbose($"Running {PluginName} plugin.");
                using (var form = new PluginMainForm())
                {
                    form.Plugin = this;
                    _= form.ShowDialog(owner);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                _ = ErrorBox.Show(owner,
                    $"There was an error while executing plugin {PluginName}:\r\n\r\n{ex.Message}");

                return false;
            }
        }

        public static ILogService LogService { get; private set; }
        protected override Guid PluginId => pluginInfo.Id;
        protected override string PluginName => pluginInfo.Name;
        public override IPluginInfo PluginInfo => pluginInfo;
        public override Image Icon => Properties.Resources.Key16;
    }
}
