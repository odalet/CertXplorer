namespace Delta.CertXplorer.DocumentModel
{
    public interface IDocumentSource
    {
        string Uri { get; }
        bool IsReadOnly { get; }
    }
}
