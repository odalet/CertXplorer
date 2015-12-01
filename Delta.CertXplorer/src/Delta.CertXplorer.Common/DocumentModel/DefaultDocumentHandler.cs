﻿using System;
using System.IO;
using Delta.CertXplorer.UI;

namespace Delta.CertXplorer.DocumentModel
{
    internal class DefaultDocumentHandler : BaseDocumentHandler<DefaultDocumentView>
    {
        public override string HandlerName
        {
            get { return "Default Document Handler [Embedded]"; }
        }

        protected override bool CanHandleSource(IDocumentSource source)
        {
            return (source != null && source is FileDocumentSource);
        }
        
        protected override IDocument CreateDocumentFromSource()
        {
            var data = File.ReadAllBytes(Source.Uri);
            var doc = new BinaryDocument();
            doc.SetData(data);

            return doc;
        }
    }
}
