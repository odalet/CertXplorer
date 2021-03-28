using System;
using System.Collections.Generic;

namespace Delta.CertXplorer.DocumentModel
{
    internal sealed class DocumentManagerService : IDocumentManagerService
    {
        private readonly IDocumentBasedUI ownerUI;
        private readonly Dictionary<string, IDocumentView> views = new();
        private readonly Func<IDocumentBasedUI, IEnumerable<IDocumentHandler>, IDocumentHandler> chooseDocumentHandler;

        public DocumentManagerService(
            IDocumentBasedUI owner,
            Func<IDocumentBasedUI, IEnumerable<IDocumentHandler>, IDocumentHandler> chooseDocumentHandlerFunction)
        {
            ownerUI = owner ?? throw new ArgumentNullException(nameof(owner));
            chooseDocumentHandler = chooseDocumentHandlerFunction;
            ownerUI.ActiveDocumentChanged += (s, e) =>
            {
                var view = ownerUI.ActiveDocumentView;
                if (view != null && view.Document != null)
                    SelectDocument(view.Document);
            };
        }

        public event DocumentEventHandler DocumentCreated;
        public event DocumentEventHandler DocumentAdded;
        public event DocumentEventHandler DocumentSelected;
        public event DocumentEventHandler DocumentRemoved;

        public IDocument CreateDocument(IDocumentSource source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var handler = FindDocumentHandler(source);
            handler.CreateDocument(source);
            var document = handler.Document;
            if (document == null)
            {
                This.Logger.Warning($"Document creation failed for source {source.Uri}");
                return null;
            }

            OnDocumentCreated(document);
            return document;
        }

        public void SelectDocument(IDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            ownerUI.ShowView(views[document.Key]);
            OnDocumentSelected(document);
        }

        public void OpenDocument(IDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (views.ContainsKey(document.Key))
            {
                SelectDocument(document);
                return;
            }

            var handler = document.Handler;
            if (handler == null)
            {
                This.Logger.Warning($"No handler could be found for document {document.Key}; reverting to default view.");
                handler = new DefaultDocumentHandler();
            }

            var view = handler.CreateView();
            if (view == null)
            {
                var error = $"Could not create a view for document {document.Key}";
                This.Logger.Error(error);
                _ = UI.ErrorBox.Show(error);
                return;
            }

            view.ViewClosed += (s, e) => CloseDocument(document, false);

            views.Add(document.Key, view);
            ownerUI.ShowView(views[document.Key]);
            OnDocumentAdded(document);
        }

        public void CloseDocument(IDocument document) => CloseDocument(document, true);
        private void CloseDocument(IDocument document, bool shouldCloseView)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (!views.ContainsKey(document.Key)) return;

            if (shouldCloseView)
            {
                var view = views[document.Key];
                CloseView(view);
            }

            _ = views.Remove(document.Key);
            OnDocumentRemoved(document);
        }

        private IDocumentHandler FindDocumentHandler(IDocumentSource source)
        {
            var registry = This.GetService<IDocumentHandlerRegistryService>();
            var found = registry.Find(source);

            if (found == null || found.Length == 0)
            {
                This.Logger.Warning($"Could not find a Document Handler for {source.Uri}");
                return null;
            }

            if (found.Length > 1)
            {
                This.Logger.Info($"Multiple Document Handlers are able to process Document {source.Uri}");
                if (chooseDocumentHandler == null)
                {
                    This.Logger.Error("No 'Choose Document Handler' function was provided to the Document Manager Service");
                    return null;
                }

                IDocumentHandler chosen = null;
                try
                {
                    chosen = chooseDocumentHandler(ownerUI, found);
                }
                catch (Exception ex)
                {
                    This.Logger.Error($"'Choose Document Handler' function raised an error: {ex.Message}", ex);
                }

                if (chosen == null) This.Logger.Warning($"No Document Handler was chosen for {source.Uri}");
                return chosen;
            }

            return found[0];
        }

        private void CloseView(IDocumentView view)
        {
            if (view == null) throw new ArgumentNullException("view");
            This.Logger.Verbose("Closing View.");
            view.Close();

            if (view is IDisposable disposable)
            {
                var document = view.Document;
                var viewInfo = document == null ? view.GetType() : document.GetType();
                This.Logger.Verbose($"Disposing view {viewInfo}");
                disposable.Dispose();
            }
        }

        private void OnDocumentCreated(IDocument document) => DocumentCreated?.Invoke(this, new DocumentEventArgs(document));
        private void OnDocumentAdded(IDocument document) => DocumentAdded?.Invoke(this, new DocumentEventArgs(document));
        private void OnDocumentSelected(IDocument document) => DocumentSelected?.Invoke(this, new DocumentEventArgs(document));
        private void OnDocumentRemoved(IDocument document) => DocumentRemoved?.Invoke(this, new DocumentEventArgs(document));
    }
}
