using System;
using Delta.CertXplorer.Asn1Decoder;
using Delta.CertXplorer.Extensibility;

namespace Delta.CertXplorer.DocumentModel
{
    internal sealed class PluginBasedDocumentHandler : BaseDocumentHandler<Asn1DocumentView>
    {
        private readonly IDataHandlerPlugin plugin;
        private IDataHandler handler = null;

        public PluginBasedDocumentHandler(IDataHandlerPlugin dataHandlerPlugin) => 
            plugin = dataHandlerPlugin ?? throw new ArgumentNullException("dataHandlerPlugin");

        public override string HandlerName => $"{plugin.PluginInfo.Name} Document Handler [Plugin]";

        protected override bool CanHandleSource(IDocumentSource source)
        {
            if (!(source is FileDocumentSource))
                return false;

            handler = plugin.CreateHandler();
            var ok = handler.CanHandleFile(((FileDocumentSource)source).Uri);

            if (ok) This.Logger.Info($"This file can be handled by plugin [{plugin.PluginInfo.Name}]");

            return ok;
        }

        protected override IDocument CreateDocumentFromSource()
        {
            if (handler == null) throw new InvalidOperationException("Inner data handler is null.");
            
            This.Logger.Info($"Running plugin [{plugin.PluginInfo.Name}] on data source {Source.Uri}");

            IData result;
            try
            {
                result = handler.ProcessFile();
            }
            catch (Exception ex)
            {
                This.Logger.Error(
                    $"Plugin [{plugin.PluginInfo.Name}] failed at processing data from source {Source.Uri}: {ex.Message}", ex);
                return null;
            }

            var doc = new PluginBasedDocument();
            doc.SetData(result.MainData);
            doc.SetAdditionalData(result.AdditionalData);
            return doc;
        }
    }
}
