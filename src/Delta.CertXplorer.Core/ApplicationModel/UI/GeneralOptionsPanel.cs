using System;
using System.Windows.Forms;

namespace Delta.CertXplorer.ApplicationModel.UI
{
    public partial class GeneralOptionsPanel : BaseOptionsPanel
    {
        public GeneralOptionsPanel() => InitializeComponent();

        protected override void OnPanelClosed(EventArgs e, DialogResult dialogResult)
        {         
            // Placeholder
        }

        public override string Text
        {
            get => SR.GeneralOptions;
            set { /* Not settable */ }
        }
    }
}
