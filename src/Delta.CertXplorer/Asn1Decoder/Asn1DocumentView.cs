using System;
using Delta.CertXplorer.DocumentModel;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer.Asn1Decoder
{
    public partial class Asn1DocumentView : DockContent, IDocumentView
    {
        public Asn1DocumentView() => InitializeComponent();

        public event EventHandler ViewClosed;

        public IDocument Document { get; private set; }

        public void SetDocument(IDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (document is not IDocumentData<byte[]> byteDocument) throw new NotSupportedException(
                $"Documents of type {Document.GetType()} are not supported in this view.");

            Document = document;
            asn1Viewer.SetData(byteDocument.Data);
            base.Text = Document.Caption;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
