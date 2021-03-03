using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// Occurs when the currently selected object has changed.
        /// </summary>
        public event EventHandler SelectionChanged;

        [DefaultValue(BorderStyle.Fixed3D)]
        public BorderStyle InnerBorderStyle
        {
            get => listView.BorderStyle;
            set => listView.BorderStyle = value;
        }

        /// <summary>
        /// Gets the currently selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject { get; private set; }

        private string CurrentStoreName { get; set; }

        private StoreLocation CurrentStoreLocation { get; set; }

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
                X509Object x509 = null;
                var certificate = GetSelectedCertificate();
                if (certificate != null) x509 = X509Object.Create(
                    certificate.X509, CurrentStoreName, CurrentStoreLocation);
                else
                {
                    var crl = GetSelectedCrl();
                    if (crl != null) x509 = X509Object.Create(
                        crl, CurrentStoreName, CurrentStoreLocation);
                    else
                    {
                        var ctl = GetSelectedCtl();
                        if (ctl != null) x509 = X509Object.Create(
                            ctl, CurrentStoreName, CurrentStoreLocation);
                    }
                }

                if (x509 != null) Commands.RunVerb(Verbs.OpenCertificate, x509);
            };

            viewInformationAction.Run += (s, ev) =>
            {
                var certificate = GetSelectedCertificate();
                if (certificate != null)
                {
                    certificate.ShowCertificateDialog(base.Handle);
                    return;
                }

                var crl = GetSelectedCrl();
                if (crl != null)
                {
                    crl.ShowCrlDialog(base.Handle);
                    return;
                }

                var ctl = GetSelectedCtl();
                if (ctl != null)
                    ctl.ShowCtlDialog(base.Handle);
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

                    // See http://msdn.microsoft.com/en-us/library/aa376559%28VS.85%29.aspx : CERT_STORE_PROV_PHYSICAL
                    //var store = new X509Store(systemStore.Name + "\\.Default", systemStore.Location.ToStoreLocation());

                    store.Open(OpenFlags.ReadOnly);

                    CurrentStoreName = store.Name;
                    CurrentStoreLocation = store.Location;

                    FillList(
                        store.GetCertificates(),
                        store.GetCertificateRevocationLists(),
                        store.GetCertificateTrustLists());
                    store.Close();
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

        private void FillList(
            IEnumerable<Certificate> certificates,
            IEnumerable<CertificateRevocationList> crls,
            IEnumerable<CertificateTrustList> ctls)
        {
            const int certificateImageIndex = 0;
            const int certificateWithPrivateKeyImageIndex = 1;
            const int crlImageIndex = 2;
            const int ctlImageIndex = 3;

            var items = certificates.Select(certificate =>
                {
                    var item = new ListViewItem(FormatDN(certificate.SubjectName));
                // TODO: don't use X509Certificate class, but the Certificate wrapper 
                // (and replace "new Certificate(certificate).IsValid" by "certificate.IsValid"
                if (!certificate.IsValid) item.ForeColor = Color.Red;
                    item.ImageIndex = certificate.HasPrivateKey ? certificateWithPrivateKeyImageIndex : certificateImageIndex;
                    item.SubItems.AddRange(new ListViewItem.ListViewSubItem[]
                    {
                    new ListViewItem.ListViewSubItem(item, FormatDN(certificate.IssuerName)),
                    new ListViewItem.ListViewSubItem(item, FormatDate(certificate.X509.NotBefore)),
                    new ListViewItem.ListViewSubItem(item, FormatDate(certificate.X509.NotAfter)),
                    new ListViewItem.ListViewSubItem(item, certificate.FriendlyName)
                    });
                    item.Tag = certificate;

                    return item;
                });

            var crlItems = crls.Select(crl =>
            {
                var item = new ListViewItem("Revocation List");
                if (!crl.IsValid) item.ForeColor = Color.Red;
                item.ImageIndex = crlImageIndex;
                item.SubItems.AddRange(
                    new ListViewItem.ListViewSubItem[]
                    {
                        new ListViewItem.ListViewSubItem(item, FormatDN(crl.IssuerName)),
                        new ListViewItem.ListViewSubItem(item, FormatDate(crl.PublicationDate)),
                        new ListViewItem.ListViewSubItem(item, FormatDate(crl.NextUpdate)),
                        new ListViewItem.ListViewSubItem(item, crl.FriendlyName)
                    });
                item.Tag = crl;

                return item;
            });

            var ctlItems = ctls.Select(ctl =>
            {
                var item = new ListViewItem("Trust List");
                if (!ctl.IsValid) item.ForeColor = Color.Red;
                item.ImageIndex = ctlImageIndex;
                item.SubItems.AddRange(
                    new ListViewItem.ListViewSubItem[]
                    {
                        new ListViewItem.ListViewSubItem(item, string.Empty),
                        new ListViewItem.ListViewSubItem(item, FormatDate(ctl.PublicationDate)),
                        new ListViewItem.ListViewSubItem(item, FormatDate(ctl.NextUpdate)),
                        new ListViewItem.ListViewSubItem(item, ctl.FriendlyName)
                    });
                item.Tag = ctl;

                return item;
            });

            listView.Items.Clear();

            foreach (var item in items) _ = listView.Items.Add(item);
            foreach (var item in crlItems) _ = listView.Items.Add(item);
            foreach (var item in ctlItems) _ = listView.Items.Add(item);
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

        private string FormatDN(X500DistinguishedName dn)
        {
            var cn = dn.ExtractRdn("cn");
            if (!string.IsNullOrEmpty(cn)) return cn;

            var ou = dn.ExtractRdn("ou");
            if (!string.IsNullOrEmpty(ou)) return ou;

            return dn.Name;
        }

        private string FormatDate(DateTime date) => date == DateTime.MinValue ? string.Empty : date.ToString("yyyy/MM/dd");

        private string FormatDate(DateTimeOffset date) => date == DateTimeOffset.MinValue ? string.Empty : date.ToString("yyyy/MM/dd");
    }
}