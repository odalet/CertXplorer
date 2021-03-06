using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Pluralsight.Crypto.UI
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms convention")]
    public partial class GenerateSelfSignedCertForm : Form
    {
        private const string pluralsightUrl = "https://www.pluralsight.com/";
        private const string blogPostUrl = "https://www.pluralsight.com/blog/software-development/selfcert-create-a-self-signed-certificate-interactively-gui-or-programmatically-in-net";

        public GenerateSelfSignedCertForm() => InitializeComponent();

        public Func<string> GetUserConfigDirectory { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadSettings();
            LoadStoreDropdownLists();
            UpdateVersion();
        }

        private void UpdateVersion() => labelVersion.Text = $"v{GetType().Assembly.GetName().Version}";

        private void LoadStoreDropdownLists()
        {
            cboStoreLocation.Items.Clear();
            foreach (StoreLocation storeLocation in Enum.GetValues(typeof(StoreLocation)))
            {
                var index = cboStoreLocation.Items.Add(storeLocation);
                if (StoreLocation.LocalMachine == storeLocation)
                    cboStoreLocation.SelectedIndex = index;
            }

            cboStoreName.Items.Clear();
            foreach (StoreName storeName in Enum.GetValues(typeof(StoreName)))
            {
                var index = cboStoreName.Items.Add(storeName);
                if (StoreName.My == storeName)
                    cboStoreName.SelectedIndex = index;
            }
        }

        private void LoadSettings()
        {
            // Defaults
            var today = DateTime.Today;

            txtDN.Text = "cn=localhost";
            cboKeySize.Text = "4096";
            dtpValidFrom.Value = today.AddDays(-7); // just to be safe
            dtpValidTo.Value = today.AddYears(10);
            chkExportablePrivateKey.Checked = true;

            XDocument doc = null;
            try
            {
                doc = XDocument.Load(SettingsFile);
            }
            catch (IOException)
            {
                // do nothing...
            }

            if (null != doc)
                LoadSettings(doc);
        }

        private void LoadSettings(XDocument doc)
        {
            txtDN.Text = GetSetting(doc, "dn", "");
            cboKeySize.Text = GetSetting(doc, "keySize", "4096");
            if (bool.TryParse(GetSetting(doc, "exportPrivateKey", "true"), out var isPrivateKeyExportable))
                chkExportablePrivateKey.Checked = isPrivateKeyExportable;
        }

        private string GetSetting(XDocument doc, string elementName, string defaultValue)
        {
            var dnElement = doc.Root.Element(elementName);
            return dnElement?.Value ?? defaultValue;
        }

        private void SaveSettings()
        {
            var doc = new XDocument(
                new XElement("settings",
                    new XElement("dn", txtDN.Text),
                    new XElement("keySize", cboKeySize.Text),
                    new XElement("exportPrivateKey", chkExportablePrivateKey.Checked)));

            doc.Save(SettingsFile);
        }

        private string SettingsFile => GetUserConfigDirectory != null ?
            Path.Combine(GetUserConfigDirectory(), "pluralsight.settings.xml") :
            Path.Combine(Application.LocalUserAppDataPath, "pluralsight.settings.xml");

        private void SaveAsPFX()
        {
            if (!ValidateCertProperties())
                return;

            var certificate = GenerateCertificate();
            if (certificate == null)
                return; // user must have cancelled the operation

            using var dialog = new SaveFileDialog { Filter = "PFX file (*.pfx)|*.pfx" };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                using (var outputStream = File.Create(dialog.FileName))
                {
                    var pfx = certificate.Export(X509ContentType.Pfx, txtPassword.Text.Length > 0 ? txtPassword.Text : null);
                    outputStream.Write(pfx, 0, pfx.Length);
                    outputStream.Close();
                }

                _ = MessageBox.Show(this,
                    $"Successfully saved a new self-signed certificate to {Path.GetFileName(dialog.FileName)}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveToCertificateStore()
        {
            var store = new X509Store((StoreName)cboStoreName.SelectedItem, (StoreLocation)cboStoreLocation.SelectedItem);
            store.Open(OpenFlags.ReadWrite);

            X509Certificate2 certificate;
            try
            {
                certificate = GenerateCertificate();
                if (null != certificate)
                {
                    // I've not been able to figure out what property isn't getting copied into the store,
                    // but IIS can't find the private key when I simply add the cert directly to the store
                    // in this fashion:  store.Add(cert).
                    // The extra two lines of code here do seem to make IIS happy though.
                    // I got this idea from here: http://www.derkeiler.com/pdf/Newsgroups/microsoft.public.inetserver.iis.security/2008-03/msg00020.pdf
                    //  (written by David Wang at blogs.msdn.com/David.Wang)
                    var pfx = certificate.Export(X509ContentType.Pfx);
                    certificate = new X509Certificate2(pfx, (string)null, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                    // NOTE: it's not clear to me at this point if this will work if you want to save to StoreLocation.CurrentUser
                    //       given that there's also an X509KeyStorageFlags.UserKeySet. That could be DPAPI related though, and not cert store related.
                    store.Add(certificate);
                }
            }
            finally
            {
                store.Close();
            }

            if (certificate != null)
            {
                using var form = new CertDetailsForm
                {
                    Certificate = certificate,
                    CertStoreLocation = (StoreLocation)cboStoreLocation.SelectedItem,
                    CertStoreName = (StoreName)cboStoreName.SelectedItem,
                };

                _ = form.ShowDialog();
            }
        }

        private X509Certificate2 GenerateCertificate()
        {
            // use a form to initiate a cancellable background worker
            using var form = new BackgroundCertGenForm
            {
                CertProperties = new SelfSignedCertProperties
                {
                    Name = new X500DistinguishedName(txtDN.Text),
                    ValidFrom = dtpValidFrom.Value,
                    ValidTo = dtpValidTo.Value,
                    KeyBitLength = int.Parse(cboKeySize.Text),
                    IsPrivateKeyExportable = true,
                }
            };

            _ = form.ShowDialog();
            return form.Certificate;
        }

        private bool ValidateCertProperties()
        {
            if (!ValidateDN())
            {
                txtDN.SelectAll();
                _ = txtDN.Focus();
                return false;
            }

            if (!ValidateKeySize())
            {
                _ = cboKeySize.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateDN()
        {
            try
            {
                _ = new X500DistinguishedName(txtDN.Text);
                errorProvider.SetError(txtDN, "");
                return true;
            }
            catch (CryptographicException ex)
            {
                errorProvider.SetError(txtDN, ex.Message);
                return false;
            }
        }

        private bool ValidateKeySize()
        {
            var error = "";
            if (int.TryParse(cboKeySize.Text, out var keySize))
            {

                switch (keySize)
                {
                    case 384:
                    case 512:
                    case 1024:
                    case 2048:
                    case 4096:
                    case 8192:
                    case 16384:
                        break;
                    default:
                        error = "Invalid key size.";
                        break;
                }
            }
            else error = "Key size must be an integer value.";

            errorProvider.SetError(cboKeySize, error);
            return string.IsNullOrEmpty(error);
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) => SaveSettings();
        private void btnSaveAsPFX_Click(object sender, EventArgs e) => SaveAsPFX();
        private void btnSaveToCertStore_Click(object sender, EventArgs e) => SaveToCertificateStore();
        private void lnkTitle_LinkClicked(object sender, MouseEventArgs e) => Process.Start(pluralsightUrl);
        private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start(blogPostUrl);
        private void cboKeySize_Validating(object sender, CancelEventArgs e) => ValidateKeySize();
        private void txtDN_Validating(object sender, CancelEventArgs e) => ValidateDN();       
    }
}
