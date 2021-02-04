using System.IO;
using Delta.CertXplorer.Asn1Decoder;

namespace Delta.CertXplorer.DocumentModel
{
    internal class Asn1DocumentHandler : BaseDocumentHandler<Asn1DocumentView>
    {
        public override string HandlerName => "ASN.1 Document Handler [Embedded]";

        protected override bool CanHandleSource(IDocumentSource source) => 
            source is X509DocumentSource || source is FileDocumentSource;

        protected override IDocument CreateDocumentFromSource()
        {
            var doc = new BinaryDocument();

            if (Source is X509DocumentSource x509Source)
            {
                var x509 = x509Source.X509Data;
                doc.SetData(x509.Data);
                return doc;
            }
            
            if (Source is FileDocumentSource fileSource)
            {
                var data = File.ReadAllBytes(fileSource.FileName);
                doc.SetData(data);
                return doc;
            }

            if (Source is null)
                This.Logger.Warning("Document Source is null");
            else
                This.Logger.Warning($"Invalid Document Source Type: {Source.GetType()}");

            return doc;
        }
    }
}
