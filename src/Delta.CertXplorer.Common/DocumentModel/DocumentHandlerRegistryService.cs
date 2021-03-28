using System;
using System.Linq;
using System.Collections.Generic;

namespace Delta.CertXplorer.DocumentModel
{
    internal sealed class DocumentHandlerRegistryService : IDocumentHandlerRegistryService
    {
        private sealed class RegistryEntry<T>
        {
            public int Priority { get; set; }
            public T Value { get; set; }
        }

        private readonly List<RegistryEntry<Func<IDocumentHandler>>> entries = new();

        public void Register(Func<IDocumentHandler> handlerBuilder, int priority = 0) => entries.Add(new RegistryEntry<Func<IDocumentHandler>>()
        {
            Priority = priority,
            Value = handlerBuilder
        });

        public IDocumentHandler[] Find(IDocumentSource source) => Find(source, true);
        public IDocumentHandler[] Find(IDocumentSource source, bool onlyKeepTopPriority)
        {
            int? foundPriority = null;
            var result = new List<IDocumentHandler>();
            foreach (var pair in entries.OrderByDescending(e => e.Priority))
            {
                var buildHandler = pair.Value;
                IDocumentHandler handler = null;               
                
                try
                {
                    handler = buildHandler();
                }
                catch (Exception ex)
                {
                    This.Logger.Error($"Handler creation error: {ex.Message}", ex);
                }

                if (handler == null)  continue;
                var canHandle = false;
                try
                {
                    canHandle = handler.CanHandle(source);
                }
                catch (Exception ex)
                {
                    This.Logger.Error($"Handler.CanHandle invocation error: {ex.Message}", ex);
                }

                if (canHandle)
                {
                    if (!foundPriority.HasValue)
                        foundPriority = pair.Priority;
                    else if (pair.Priority < foundPriority && onlyKeepTopPriority)
                        break;

                    result.Add(handler);
                }
            }

            return result.Count == 0 ? 
                new IDocumentHandler[] { new DefaultDocumentHandler() } : 
                result.ToArray();
        }
    }
}
