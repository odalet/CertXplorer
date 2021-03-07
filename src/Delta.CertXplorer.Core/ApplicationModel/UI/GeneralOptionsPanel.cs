using System;
using System.Windows.Forms;

namespace Delta.CertXplorer.ApplicationModel.UI
{
    /// <summary>
    /// This is the default (and unique for now) options panel.
    /// </summary>
    public partial class GeneralOptionsPanel : BaseOptionsPanel
    {
        /// <summary>
        /// Called when the <see cref="OptionsDialog"/> raises the <see cref="E:Closed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <param name="dialogResult">The parent <see cref="OptionsDialog"/> dialog result.</param>
        protected override void OnPanelClosed(EventArgs e, DialogResult dialogResult)
        {         
            // Placeholder
        }

        public override string Text
        {
            get => SR.GeneralOptions;
            set { /* Not settable */ }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOptionsPanel"/> class.
        /// </summary>
        public GeneralOptionsPanel() => InitializeComponent();

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;
        }
    }
}
