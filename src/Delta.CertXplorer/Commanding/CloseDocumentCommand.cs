using Delta.CertXplorer.DocumentModel;

namespace Delta.CertXplorer.Commanding
{
    internal sealed class CloseDocumentCommand : BaseCommand<IDocument>
    {
        public CloseDocumentCommand() : base() { }

        public override string Name => "Close Document";

        protected override void RunCommand()
        {
            if (Target != null)
                This.GetService<IDocumentManagerService>(true).CloseDocument(Target);
        }
    }
}
