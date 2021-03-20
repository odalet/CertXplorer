using Delta.CertXplorer.UI;

namespace Delta.CertXplorer.CertManager
{
    internal partial class CertificateStoreTreeView : TreeViewEx
    {
        public CertificateStoreTreeView()
        {
            InitializeComponent();
            base.AllowDrop = false;
        }        
    }
}
