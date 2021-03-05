using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Delta.CapiNet;
using Delta.CertXplorer.CertManager.Wrappers;
using Delta.CertXplorer.Commanding;
using Delta.CertXplorer.DocumentModel;
using Delta.CertXplorer.Services;
using Delta.CertXplorer.UI;
using Delta.CertXplorer.UI.Actions;
using Delta.CertXplorer.UI.Theming;

namespace Delta.CertXplorer.CertManager
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms convention")]
    public partial class CertificateListControl : ServicedUserControl, ISelectionSource
    {
        private object parentObject = null;
        private UIAction defaultAction = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateListControl"/> class.
        /// </summary>
        public CertificateListControl()
        {
            InitializeComponent();

            ThemesManager.RegisterThemeAwareControl(this, (renderer) =>
            {
                if (renderer is ToolStripProfessionalRenderer tsp)
                    tsp.RoundedEdges = false;
                tstrip.Renderer = renderer;
            });

            tstrip.SetRoundedEdges(false);

            InitializeViewMenu();
            UpdateViewMenu();
        }

        public event EventHandler SelectionChanged;

        [DefaultValue(BorderStyle.Fixed3D)]
        public BorderStyle InnerBorderStyle
        {
            get => listView.BorderStyle;
            set => listView.BorderStyle = value;
        }

        public object SelectedObject { get; private set; }
        private string CurrentStoreName { get; set; }
        private StoreLocation CurrentStoreLocation { get; set; }

        private IEnumerable<Certificate> Certificates { get; set; }
        private IEnumerable<CertificateRevocationList> CertificateRevocationLists { get; set; }
        private IEnumerable<CertificateTrustList> CertificateTrustLists { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;

            CreateAndBindToSelectionService();
            listView.SelectedIndexChanged += (s, ee) =>
            {
                if (listView.SelectedItems.Count == 0) NotifySelectionChanged(null);
                else
                {
                    var selectedItem = listView.SelectedItems[0];
                    NotifySelectionChanged(
                        ObjectWrapper.Wrap(selectedItem.Tag));
                }
            };

            // Actions
            defaultAction = openCertificateAction;
            listView.MouseDoubleClick += (s, ev) => defaultAction.DoRun();

            openCertificateAction.Run += (s, ev) =>
            {
                X509Object retrieveSelection()
                {
                    var certificate = GetSelectedCertificate();
                    if (certificate != null)
                        return X509Object.Create(certificate.X509, CurrentStoreName, CurrentStoreLocation);

                    var crl = GetSelectedCrl();
                    if (crl != null)
                        return X509Object.Create(crl, CurrentStoreName, CurrentStoreLocation);

                    var ctl = GetSelectedCtl();
                    if (ctl != null)
                        return X509Object.Create(ctl, CurrentStoreName, CurrentStoreLocation);

                    return null;
                }

                var x509 = retrieveSelection();
                if (x509 != null) Commands.RunVerb(Verbs.OpenCertificate, x509);
            };

            viewInformationAction.Run += (s, ev) =>
            {
                var certificate = GetSelectedCertificate();
                if (certificate != null)
                {
                    certificate.ShowCertificateDialog(Handle);
                    return;
                }

                var crl = GetSelectedCrl();
                if (crl != null)
                {
                    crl.ShowCrlDialog(Handle);
                    return;
                }

                var ctl = GetSelectedCtl();
                if (ctl != null)
                    ctl.ShowCtlDialog(Handle);
            };
        }

        private void InitializeViewMenu()
        {
            tileToolStripMenuItem.Tag = View.Tile;
            largeIconsToolStripMenuItem.Tag = View.LargeIcon;
            smallIconsToolStripMenuItem.Tag = View.SmallIcon;
            listToolStripMenuItem.Tag = View.List;
            detailsToolStripMenuItem.Tag = View.Details;

            foreach (ToolStripMenuItem item in viewDropDownButton.DropDownItems) item.Click += (s, e) =>
            {
                var menuItem = (ToolStripMenuItem)s;
                listView.View = (View)menuItem.Tag;
                listView.HeaderStyle = ColumnHeaderStyle.Clickable;
                UpdateViewMenu();
            };

        }

        private void UpdateViewMenu()
        {
            foreach (ToolStripMenuItem item in viewDropDownButton.DropDownItems)
                item.Checked = listView.View == (View)item.Tag;
        }

        private void CreateAndBindToSelectionService()
        {
            var service = GlobalSelectionService.GetOrCreateSelectionService(Services);
            service.SelectionChanged += (s, e) =>
            {
                if (service.CurrentSource == this) return;
                if (service.SelectedObject == null) return;
                if (service.SelectedObject == parentObject) return;

                if (service.SelectedObject is CertificateStore)
                {
                    parentObject = service.SelectedObject;
                    var systemStore = (CertificateStore)parentObject;
                    var store = systemStore.GetX509Store();
                    store.Open(OpenFlags.ReadOnly);

                    CurrentStoreName = store.Name;
                    CurrentStoreLocation = store.Location;

                    Certificates = store.GetCertificates();
                    CertificateRevocationLists = store.GetCertificateRevocationLists();
                    CertificateTrustLists = store.GetCertificateTrustLists();

                    store.Close();
                    RefreshListView();
                }
            };

            service.AddSource(this);
        }

        private void NotifySelectionChanged(object selection)
        {
            if (SelectedObject == selection) return;
            SelectedObject = selection;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RefreshListView()
        {
            const int certificateImageIndex = 0;
            const int certificateWithPrivateKeyImageIndex = 1;
            const int crlImageIndex = 2;
            const int ctlImageIndex = 3;

            // Transform certificates, crls and ctls into something close to the resulting ListViewItems
            var x509Items =
                Certificates.Select(c => new X509Item(c)).Concat(
                CertificateRevocationLists.Select(c => new X509Item(c))).Concat(
                CertificateTrustLists.Select(c => new X509Item(c)));

            // Filter if needed
            if (!string.IsNullOrEmpty(filterBox.Text))
                x509Items = x509Items.Where(item => item.Match(filterBox.Text));

            var items = x509Items.Select(x509item =>
            {
                var item = new ListViewItem(x509item.IssuedTo);
                if (!x509item.IsValid) item.ForeColor = Color.Red;
                item.ImageIndex = item.Tag switch
                {
                    Certificate => x509item.HasPrivateKey ? certificateWithPrivateKeyImageIndex : certificateImageIndex,
                    CertificateRevocationList => crlImageIndex,
                    CertificateTrustList => ctlImageIndex,
                    _ => 0
                };

                item.Tag = x509item.Tag;
                item.SubItems.AddRange(new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, x509item.IssuedBy),
                    new ListViewItem.ListViewSubItem(item, x509item.From),
                    new ListViewItem.ListViewSubItem(item, x509item.To),
                    new ListViewItem.ListViewSubItem(item, x509item.FriendlyName)
                });

                return item;
            });

            listView.Items.Clear();
            foreach (var item in items) _ = listView.Items.Add(item);
        }

        private Certificate GetSelectedCertificate()
        {
            if (listView.SelectedItems.Count == 0) return null;

            var tag = listView.SelectedItems[0].Tag;
            if (tag is Certificate certificate) return certificate;
            return null;
        }

        private CertificateRevocationList GetSelectedCrl()
        {
            if (listView.SelectedItems.Count == 0) return null;

            var tag = listView.SelectedItems[0].Tag;
            if (tag is CertificateRevocationList crl) return crl;
            return null;
        }

        private CertificateTrustList GetSelectedCtl()
        {
            if (listView.SelectedItems.Count == 0) return null;

            var tag = listView.SelectedItems[0].Tag;
            if (tag is CertificateTrustList ctl) return ctl;
            return null;
        }

        private void filterBox_TextChanged(object sender, EventArgs e) => RefreshListView();
    }
}