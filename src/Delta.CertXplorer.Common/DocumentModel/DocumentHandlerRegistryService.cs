using System;
using System.Linq;
using System.Collections.Generic;

namespace Delta.CertXplorer.DocumentModel
{
    internal class DocumentHandlerRegistryService : IDocumentHandlerRegistryService //: IDocumentBuilderRegistryService, IViewBuilderRegistryService
    {
        public class RegistryEntry<T>
        {
            public int Priority { get; set; }
            public T Value { get; set; }
        }

        private List<RegistryEntry<Func<IDocumentHandler>>> list = new List<RegistryEntry<Func<IDocumentHandler>>>();


        #region IDocumentHandlerRegistryService Members

        public void Register(Func<IDocumentHandler> handlerConstructor, int priority = 0)
        {
            var entry = new RegistryEntry<Func<IDocumentHandler>>()
            {
                Priority = priority,
                Value = handlerConstructor
            };

            list.Add(entry);
        }

        public IDocumentHandler[] Find(IDocumentSource source)
        {
            return Find(source, true);
        }

        public IDocumentHandler[] Find(IDocumentSource source, bool onlyKeepTopPriority)
        {
            int? foundPriority = null;
            var result = new List<IDocumentHandler>();
            foreach (var pair in list.OrderByDescending(e => e.Priority))
            {
                var function = pair.Value;
                IDocumentHandler handler = null;               
                
                try
                {
                    handler = function();
                }
                catch (Exception ex)
                {
                    This.Logger.Error(string.Format("Handler creation error: {0}", ex.Message), ex);
                }

                if (handler == null)  continue;
                var canHandle = false;
                try
                {
                    canHandle = handler.CanHandle(source);
                }
                catch (Exception ex)
                {
                    This.Logger.Error(string.Format("Handler.CanHandle invocation error: {0}", ex.Message), ex);
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

            if (result.Count == 0)
                return new IDocumentHandler[] { new DefaultDocumentHandler() };
            else return result.ToArray();
        }

        #endregion
    }
}
