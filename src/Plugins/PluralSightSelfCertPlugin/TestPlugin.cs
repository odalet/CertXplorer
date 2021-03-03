using System;
using System.Windows.Forms;

using Delta.CertXplorer.Extensibility;

namespace PluralSightSelfCertPlugin
{
    internal sealed class TestPlugin : BaseGlobalPlugin
    {
        private static readonly Guid guid = new Guid("{74BAA351-6DC2-4677-B01F-4B6D62428808}");

        protected override Guid PluginId => guid;
        protected override string PluginName => "Test plugin";

        /// <summary>
        /// Called when the plugin is initialized.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (MessageBox.Show("Throw?", "?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                throw new ApplicationException("Test Exception");
            else Log.Info("Did not throw!");
        }

        /// <summary>
        /// Runs this plugin passing it the specified Windows forms parent object.
        /// </summary>
        /// <param name="owner">The Windows forms parent object.</param>
        /// <returns>
        /// 	<c>true</c> if the execution was successful; otherwise, <c>false</c>.
        /// </returns>
        public override bool Run(IWin32Window owner)
        {
            Log.Debug("Test plugin!");
            return true;
        }
    }
}
