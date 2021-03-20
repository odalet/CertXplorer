using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Delta.CertXplorer.CertManager;
using Delta.CertXplorer.Commanding;
using Delta.CertXplorer.DocumentModel;
using Delta.CertXplorer.Services;
using Delta.CertXplorer.UI;

namespace Delta.CertXplorer.Asn1Decoder
{
    public partial class DocumentManagerControl : ServicedUserControl, ISelectionSource
    {
        private sealed class MenuProvider : ITreeViewExContextMenuStripProvider
        {
            private readonly ContextMenuStrip menuStrip;

            public MenuProvider(ContextMenuStrip cm) => menuStrip = cm;

            public ContextMenuStrip GetContextMenuStrip(TreeNodeEx node) => node.Tag is IDocument ? menuStrip : null;
        }

        private const int documentsIndex = 0;
        private const int closedFolderIndex = 1;
        private const int openedFolderIndex = 2;
        private const int documentIndex = 3;

        private readonly Dictionary<IDocument, TreeNode> documents;
        private IServiceProvider services = null;
        private TreeNode filesRoot = null;
        private TreeNode certificatesRoot = null;

        public DocumentManagerControl()
        {
            InitializeComponent();

            documents = new Dictionary<IDocument, TreeNode>();
            mstrip.Renderer = VS2015ThemeProvider.Renderer;
            tvExplorer.ContextMenuStripProvider = new MenuProvider(mstrip);
        }

        public event EventHandler SelectionChanged;

        public object SelectedObject => SelectedDocument;

        private IDocument SelectedDocument => tvExplorer.SelectedNode?.Tag as IDocument;

        public void Initialize(IServiceProvider serviceProvider)
        {
            services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            var documentManager = services.GetService<IDocumentManagerService>(true);
            documentManager.DocumentAdded += (s, e) => OnDocumentAdded(s, e);
            documentManager.DocumentRemoved += (s, e) => OnDocumentRemoved(s, e);
            documentManager.DocumentSelected += (s, e) => OnDocumentSelected(s, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;

            var root = new TreeNode("Documents")
            {
                ImageIndex = documentsIndex
            };

            filesRoot = CreateFolderTreeNode("Files");
            certificatesRoot = CreateFolderTreeNode("Certificates");

            _ = root.Nodes.Add(filesRoot);
            _ = root.Nodes.Add(certificatesRoot);
            _ = tvExplorer.Nodes.Add(root);
            root.Expand();

            GlobalSelectionService
                .GetOrCreateSelectionService(Services)
                .AddSource(this);

            openAction.Run += (s, ev) =>
            {
                var doc = SelectedDocument;
                if (doc != null) Commands.RunVerb(Verbs.OpenExisting, doc);
            };

            closeAction.Run += (s, ev) =>
            {
                var doc = SelectedDocument;
                if (doc != null) Commands.RunVerb(Verbs.CloseDocument, doc);
            };

            tvExplorer.SelectedNodeChanged += (s, ev) => SelectionChanged?.Invoke(this, EventArgs.Empty);
            tvExplorer.NodeMouseDoubleClick += (s, ev) =>
            {
                if (ev.Node.Tag is IDocument doc) Commands.RunVerb(Verbs.OpenExisting, doc);
            };

            tvExplorer.DragDrop += (s, ev) =>
            {
                var docList = (string[])ev.Data.GetData(DataFormats.FileDrop);
                if (docList.Length != 0)
                {
                    foreach (var doc in docList)
                    {
                        if (File.Exists(doc))
                            Commands.RunVerb(Verbs.OpenFile, doc);
                    }
                }
            };

            tvExplorer.DragOver += (s, ev) => ev.Effect = DragDropEffects.Copy;
        }

        private FolderTreeNode CreateFolderTreeNode(string text) => new(text)
        {
            CollapsedImageIndex = closedFolderIndex,
            SelectedCollapsedImageIndex = closedFolderIndex,
            ExpandedImageIndex = openedFolderIndex,
            SelectedExpandedImageIndex = openedFolderIndex
        };

        private void OnDocumentSelected(object sender, DocumentEventArgs e)
        {
            if (!documents.ContainsKey(e.Document)) return;
            tvExplorer.SelectedNode = documents[e.Document];
        }

        private void OnDocumentRemoved(object sender, DocumentEventArgs e)
        {
            if (!documents.ContainsKey(e.Document)) return;

            var node = documents[e.Document];
            _ = documents.Remove(e.Document);
            filesRoot.Nodes.Remove(node);
        }

        private void OnDocumentAdded(object sender, DocumentEventArgs e)
        {
            if (documents.ContainsKey(e.Document)) return;

            var caption = "Document";
            if (e.Document is IDocument document)
                caption = document.Caption;

            var tn = new TreeNodeEx(caption)
            {
                Tag = e.Document,
                ImageIndex = documentIndex,
                SelectedImageIndex = documentIndex
            };

            documents.Add(e.Document, tn);

            var tnRoot = e.Document.Source is X509DocumentSource ? certificatesRoot : filesRoot;
            _ = tnRoot.Nodes.Add(tn);
            tnRoot.Expand();
        }
    }
}
