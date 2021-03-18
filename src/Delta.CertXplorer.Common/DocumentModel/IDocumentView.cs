using Delta.CertXplorer.UI;

namespace Delta.CertXplorer.DocumentModel
{
    /// <summary>
    /// Represents the UI displaying a document.
    /// </summary>
    public interface IDocumentView : IView
    {
        void SetDocument(IDocument document);
        IDocument Document { get; }
    }
}
