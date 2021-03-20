using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Delta.CapiNet;

namespace Delta.CertXplorer.Asn1Decoder
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms conventions")]
    public partial class CertificateStoreChooserDialog : Form
    {
        public interface ICertificateStoreChooserValue
        {
            string StoreName { get; }
            StoreLocation StoreLocation { get; }
            X509Certificate2Collection GetCertificates();
        }

        private sealed class CertificateStoreChooserValue : ICertificateStoreChooserValue
        {
            public CertificateStoreChooserValue(StoreName name, StoreLocation location)
            {
                StoreName = name.ToString();
                StoreLocation = location;
            }

            public string StoreName { get; }
            public StoreLocation StoreLocation { get; }

            public string NameWithCount => $"{StoreName} [{CertificateCount}]";

            public int CertificateCount => Execute(store => store.Certificates.Count, StoreName, StoreLocation);

            public X509Certificate2Collection GetCertificates() => Execute(store => store.Certificates, StoreName, StoreLocation);

            private T Execute<T>(Func<X509Store, T> function, string storeName, StoreLocation storeLocation)
            {
                var store = Capi32.GetCertificateStore(storeName, storeLocation);
                var x509Store = store.GetX509Store();

                x509Store.Open(OpenFlags.ReadOnly);
                var result = default(T);
                try
                {
                    result = function(x509Store);
                }
                finally 
                {
                    x509Store.Close(); 
                }

                return result;
            }
        }

        private TreeNode currentUserTreeNode;
        private TreeNode localMachineTreeNode;

        public CertificateStoreChooserDialog() => InitializeComponent();

        public ICertificateStoreChooserValue SelectedValue { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Cursor = Cursors.WaitCursor;
            try
            {
                currentUserTreeNode = new TreeNode("Current User");
                _ = tvStores.Nodes.Add(currentUserTreeNode);

                localMachineTreeNode = new TreeNode("Local Machine");
                _ = tvStores.Nodes.Add(localMachineTreeNode);

                foreach (StoreName name in Enum.GetValues(typeof(StoreName)))
                {
                    CreateNode(name, StoreLocation.CurrentUser);
                    CreateNode(name, StoreLocation.LocalMachine);
                }

                currentUserTreeNode.Expand();
                localMachineTreeNode.Expand();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void CreateNode(StoreName name, StoreLocation location)
        {
            var cscv = new CertificateStoreChooserValue(name, location);
            if (cscv.CertificateCount <= 0) return;

            var rootNode = location == StoreLocation.CurrentUser ? currentUserTreeNode : localMachineTreeNode;
            _ = rootNode.Nodes.Add(new TreeNode(cscv.NameWithCount) { Tag = cscv });
        }

        private void tvStores_AfterSelect(object sender, TreeViewEventArgs e) => 
            SelectedValue = e.Node.Tag as CertificateStoreChooserValue;

        private void tvStores_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (SelectedValue == null) return;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}