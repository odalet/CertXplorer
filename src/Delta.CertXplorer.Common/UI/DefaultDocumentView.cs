using System;
using Delta.CertXplorer.DocumentModel;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer.UI
{
    public partial class DefaultDocumentView : DockContent, IDocumentView
    {
        public DefaultDocumentView() => InitializeComponent();

        public void SetDocument(IDocument document)
        {
            if (Document != null) throw new InvalidOperationException("This view's document can only be set once.");
            Document = document ?? throw new ArgumentNullException(nameof(document));

            UpdateInfo();
        }

        public IDocument Document { get; private set; }

        public event EventHandler ViewClosed;

        protected virtual void UpdateInfo()
        {
            Text = Document.Caption;
            documentCaptionBox.Text = Document.Caption;
            documentKeyBox.Text = Document.Key;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
