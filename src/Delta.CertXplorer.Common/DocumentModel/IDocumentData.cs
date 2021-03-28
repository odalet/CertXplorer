
namespace Delta.CertXplorer.DocumentModel
{
    public interface IDocumentData<out T>
    {
        T Data { get; }
    }

    public interface IDocumentData : IDocumentData<object> { }
}
