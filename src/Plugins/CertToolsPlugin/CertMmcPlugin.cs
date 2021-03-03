using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Delta.CertXplorer.Extensibility;
using Delta.CertXplorer.Extensibility.UI;

namespace CertToolsPlugin
{
    internal class CertMmcPlugin : BaseGlobalPlugin
    {
        private static readonly CertMmcPluginInfo pluginInfo = new CertMmcPluginInfo();

        public override IPluginInfo PluginInfo => pluginInfo;
        public override Image Icon => Properties.Resources.mmc16;
        protected override Guid PluginId => pluginInfo.Id;
        protected override string PluginName => pluginInfo.Name;

        public override bool Run(IWin32Window owner)
        {
            try
            {
                Log.Verbose($"Running {PluginName} plugin.");

                // Get the user data directory.
                string userDataDir = null;
                try
                {
                    var host = Services.GetService<IHostService>(true);
                    userDataDir = host.UserDataDirectory;
                }
                catch (Exception ex)
                {
                    Log.Warning($"Could not retrieve the User Data Directory; falling back to the System temporary data directory: {ex.Message}", ex);
                }

                try
                {
                    if (userDataDir == null)
                        userDataDir = Path.GetTempPath();
                    if (File.Exists(userDataDir))
                        File.Delete(userDataDir);
                    if (!Directory.Exists(userDataDir))
                        _ = Directory.CreateDirectory(userDataDir);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    _ = ErrorBox.Show(owner,
                        $"There was an error while executing plugin {PluginName}:\r\n\r\n{ex.Message}");

                    return false;
                }

                try
                {
                    var targetFile = Path.Combine(userDataDir, "certs.msc");
                    if (!File.Exists(targetFile))
                    {
                        var msc = CertificateMmcTemplates.MscTemplate;
                        File.WriteAllText(targetFile, msc);
                    }

                    _ = Process.Start(targetFile);
                }
                catch (Win32Exception wex)
                {
                    if ((uint)wex.ErrorCode == 0x80004005) // operation was canceled by the user.
                    {
                        Log.Info(wex.Message);
                        return true;
                    }
                    else throw;
                }
                catch (Exception ex)
                {
                    var message = $"Could not run the certificates console: {ex.Message}";
                    Log.Error(message, ex);
                    _ = ErrorBox.Show(owner, message);

                    return false;
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
