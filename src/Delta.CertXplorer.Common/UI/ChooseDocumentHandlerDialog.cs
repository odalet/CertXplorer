using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Delta.CertXplorer.DocumentModel;

namespace Delta.CertXplorer.UI
{
    internal partial class ChooseDocumentHandlerDialog : Form
    {
        private IEnumerable<IDocumentHandler> handlers;
        private readonly List<RadioButton> buttons = new();

        public ChooseDocumentHandlerDialog()
        {
            InitializeComponent();
            okButton.Enabled = false;
        }

        public IDocumentHandler SelectedDocumentHandler =>
            buttons.SingleOrDefault(b => b.Checked)?.Tag as IDocumentHandler;

        public void SetDocumentHandlers(IEnumerable<IDocumentHandler> documentHandlers)
        {
            handlers = documentHandlers;

            foreach (var control in buttons)
                contentPanel.Controls.Remove(control);
            buttons.Clear();

            contentPanel.SuspendLayout();

            buttons.AddRange(handlers.Select(h =>
            {
                var rb = new RadioButton()
                {
                    Text = h.HandlerName,
                    Font = new Font("Segoe UI", 12F),
                    Tag = h,
                    AutoSize = false,
                    AutoEllipsis = true,
                    Height = 30,
                    Dock = DockStyle.Top,
                };

                rb.CheckedChanged += (s, e) => okButton.Enabled = buttons.Any(b => b.Checked);

                contentPanel.Controls.Add(rb);
                return rb;
            }));

            contentPanel.ResumeLayout(true);
        }
    }
}
