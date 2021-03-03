using System;
using System.Windows.Forms;
using Delta.CertXplorer.ApplicationModel;

namespace Delta.CertXplorer.About
{
    public partial class AboutCertXplorerForm : WindowsFormsAboutServiceForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutDelta.CertXplorerForm"/> class.
        /// </summary>
        public AboutCertXplorerForm()
        {
            InitializeComponent();

            Text = "About " + Program.ApplicationName;

            var service = This.GetService<ILayoutService>();
            if (service != null) service.RegisterForm("Delta.CertXplorer.About", this);
        }

        /// <summary>
        /// Raises the <see cref="E:Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var page = new TabPage("Plugins");
            page.Controls.Add(new AboutPluginsControl
            {
                Dock = DockStyle.Fill
            });

            Tabs.TabPages.Add(page);
        }
    }
}
