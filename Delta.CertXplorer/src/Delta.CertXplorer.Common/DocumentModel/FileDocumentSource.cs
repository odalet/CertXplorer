using System.IO;

namespace Delta.CertXplorer.DocumentModel
{
    public sealed class FileDocumentSource : IDocumentSource
    {
        public FileDocumentSource(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("Input file does not exist.", filename ?? string.Empty);
            FileName = filename;
        }

        public string FileName { get; }
        public string Uri => FileName;
        public bool IsReadOnly => true;
    }
}
