using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Delta.CapiNet;
using Delta.CertXplorer.Services;
using Delta.CertXplorer.UI;
using Delta.CertXplorer.UI.Theming;

namespace Delta.CertXplorer.CertManager
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms convention")]
    public partial class CertificateStoreListControl : ServicedUserControl, ISelectionSource
    {
        private const int STORES_IMAGE = 0;
        private const int CLOSED_LOCATION_IMAGE = 1;
        private const int OPENED_LOCATION_IMAGE = 2;
        private const int CLOSED_STORE_IMAGE = 1;
        private const int OPENED_STORE_IMAGE = 2;

        private TreeNodeEx rootNode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateStoreListControl"/> class.
        /// </summary>
        public CertificateStoreListControl()
        {
            InitializeComponent();

            ShowPhysicalStores = false; // default value
            ShowOtherLocations = false; // default value

            ThemesManager.RegisterThemeAwareControl(this, (renderer) =>
            {
                if (renderer is ToolStripProfessionalRenderer tspRenderer)
                    tspRenderer.RoundedEdges = false;
                tstrip.Renderer = renderer;
            });

            tstrip.SetRoundedEdges(false);
        }

        public event EventHandler SelectionChanged;

        public object SelectedObject { get; private set; }

        [DefaultValue(BorderStyle.Fixed3D)]
        public BorderStyle InnerBorderStyle
        {
            get => tvStores.BorderStyle;
            set => tvStores.BorderStyle = value;
        }

        [DefaultValue(false)]
        public bool ShowPhysicalStores { get; set; }

        [DefaultValue(false)]
        public bool ShowOtherLocations { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;

            GlobalSelectionService.GetOrCreateSelectionService(Services).AddSource(this);
            FillNodes();

            tvStores.SelectedNodeChanged += (s, ev) => NotifySelectionChanged(ev.Node.Tag);
        }

        private void FillNodes()
        {
            // The nodes
            rootNode = new TreeNodeEx(SR.CertificateStores)
            {
                ImageIndex = STORES_IMAGE,
                SelectedImageIndex = STORES_IMAGE
            };

            _ = tvStores.Nodes.Add(rootNode);

            // 1st level: locations
            var locations = ShowOtherLocations ?
                CertificateStoreLocation.GetSystemStoreLocations() :
                new StoreLocation[]
                {
                    StoreLocation.LocalMachine,
                    StoreLocation.CurrentUser
                }.Select(l => CertificateStoreLocation.FromStoreLocation(l));

            rootNode.Nodes
                .AddRange(locations
                .OrderBy(location => location.Id)
                .Select(location =>
                {
                    var locationNode = new FolderTreeNode(location.ToString())
                    {
                        CollapsedImageIndex = CLOSED_LOCATION_IMAGE,
                        SelectedCollapsedImageIndex = CLOSED_LOCATION_IMAGE,
                        ExpandedImageIndex = OPENED_LOCATION_IMAGE,
                        SelectedExpandedImageIndex = OPENED_LOCATION_IMAGE,
                        Tag = location
                    };

                    // 2nd level: stores
                    locationNode.Nodes
                        .AddRange(Capi32.GetSystemStores(location)
                        .Select(store =>
                        {
                            var storeNode = new FolderTreeNode(store.ToLongString())
                            {
                                CollapsedImageIndex = CLOSED_STORE_IMAGE,
                                SelectedCollapsedImageIndex = CLOSED_STORE_IMAGE,
                                ExpandedImageIndex = OPENED_STORE_IMAGE,
                                SelectedExpandedImageIndex = OPENED_STORE_IMAGE,
                                Tag = store
                            };

                            // 3rd level: physical stores
                            if (ShowPhysicalStores) storeNode.Nodes.AddRange(
                                Capi32.GetPhysicalStores(store.Name)
                                .Select(pstore =>
                                {
                                    var label = $"{pstore} [{Capi32.LocalizeName(pstore)}]";
                                    var pstoreNode = new TreeNodeEx(label)
                                    {
                                        ImageIndex = CLOSED_STORE_IMAGE,
                                        SelectedImageIndex = OPENED_STORE_IMAGE,
                                        Tag = store
                                    };

                                    return pstoreNode;
                                }).ToArray());

                            return storeNode;
                        }).ToArray());

                    return locationNode;
                }).ToArray());

            rootNode.Expand();
        }

        private void NotifySelectionChanged(object selection)
        {
            if (SelectedObject == selection) return;
            SelectedObject = selection;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void refreshToolStripButton_Click(object sender, EventArgs e) => CapiNet.UI.ShowBuildCtlWizard(Handle);
    }
}
