using System;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace Pluralsight.Crypto.UI
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms convention")]
    public partial class CertDetailsForm : Form
    {
        public CertDetailsForm() => InitializeComponent();
        public StoreLocation CertStoreLocation { get; set; }
        public StoreName CertStoreName { get; set; }
        public X509Certificate2 Certificate { get; set; }

        private void CertDetailsForm_Load(object sender, EventArgs e)
        {
            txtStore.Text = $"{CertStoreLocation}/{CertStoreName}";
            txtThumbprint.Text = Certificate.Thumbprint;

            if (Certificate.PrivateKey is RSACryptoServiceProvider privateKey)
            {
                var keyFile = privateKey.CspKeyContainerInfo.UniqueKeyContainerName;
                txtPrivateKeyFile.Text = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    @"Microsoft\Crypto\RSA\MachineKeys",
                    keyFile);
            }
            else btnViewPrivateKeyFile.Enabled = false;
        }

        private void btnViewStore_Click(object sender, EventArgs e)
        {
            var store = new X509Store(CertStoreName, CertStoreLocation);
            store.Open(OpenFlags.ReadOnly);

            _ = X509Certificate2UI.SelectFromCollection(
                store.Certificates, txtStore.Text, "", X509SelectionFlag.SingleSelection);
        }

        private void btnViewCert_Click(object sender, EventArgs e) => X509Certificate2UI.DisplayCertificate(Certificate);

        private void btnViewPrivateKeyFile_Click(object sender, EventArgs e) => Process.Start(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"),
            $"/select,{txtPrivateKeyFile.Text}");
    }
}
