using System.Windows.Forms;
using Delta.CertXplorer.DocumentModel;

namespace Delta.CertXplorer.Commanding
{
    internal sealed class OpenFileCommand : BaseOpenDocumentCommand<string>
    {
        public OpenFileCommand() : base() { }

        public override string Name => "Open File Document";

        protected override IDocument OpenDocument(object[] arguments)
        {
            var fileName = string.Empty;
            if (arguments != null && arguments.Length > 0 && arguments[0] is string arg)
                fileName = arg;

            if (string.IsNullOrEmpty(fileName))
            {
                using var dialog = new OpenFileDialog();
                if (dialog.ShowDialog(Globals.MainForm) == DialogResult.OK)
                    fileName = dialog.FileName;
            }

            if (string.IsNullOrEmpty(fileName)) return null;

            var manager = This.GetService<IDocumentManagerService>(true);
            var source = new FileDocumentSource(fileName);
            return manager.CreateDocument(source);
        }
    }
}
