using Delta.CertXplorer.DocumentModel;

namespace Delta.CertXplorer.Commanding
{
    internal sealed class OpenExistingDocumentCommand : BaseOpenDocumentCommand<IDocument>
    {
        public OpenExistingDocumentCommand() : base() { }

        public override string Name => "Open Existing Document";

        protected override IDocument OpenDocument(object[] arguments) => (IDocument)arguments[0];
    }
}
