namespace Delta.CertXplorer.DocumentModel
{
    internal static class DocumentModelExtensions
    {
        public static void RegisterHandlerPlugin(this IDocumentHandlerRegistryService registry, PluginBasedDocumentHandler handler) => 
            registry.Register(() => handler, int.MaxValue);
    }
}
