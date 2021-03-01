using System;
using System.Drawing;
using System.Windows.Forms;
using Delta.CertXplorer.Extensibility;
using Delta.CertXplorer.Extensibility.UI;
using Pluralsight.Crypto.UI;

namespace PluralSightSelfCertPlugin
{
    internal sealed class SelfSignedCertificatePlugin : BaseGlobalPlugin
    {
        private static readonly SelfSignedCertificatePluginInfo pluginInfo = new SelfSignedCertificatePluginInfo();

        public override IPluginInfo PluginInfo => pluginInfo;
        public override Image Icon => Properties.Resources.pluralsight;
        protected override Guid PluginId => pluginInfo.Id;
        protected override string PluginName => pluginInfo.Name;

        public override bool Run(IWin32Window owner)
        {
            try
            {
                Log.Verbose($"Running {PluginName} plugin.");

                // Get the user config directory.
                string userConfigDir = null;
                try
                {
                    var host = Services.GetService<IHostService>(true);
                    userConfigDir = host.UserConfigDirectory;
                }
                catch (Exception ex)
                {
                    Log.Error($"Could not retrieve the User Configuration Directory: {ex.Message}", ex);
                }

                using (var form = new GenerateSelfSignedCertForm())
                {
                    if (userConfigDir != null)
                        form.GetUserConfigDirectory = () => userConfigDir;
                            
                    _ = form.ShowDialog(owner);
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
    }
}
