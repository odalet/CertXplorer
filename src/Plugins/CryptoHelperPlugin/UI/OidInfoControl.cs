using System;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace CryptoHelperPlugin.UI
{
    public partial class OidInfoControl : UserControl
    {
        // TODO: use other OID information providers:
        // http://www.alvestrand.no/objectid/1.2.840.113549.1.1.10.html + 
        // http://www.alvestrand.no/objectid/submissions/0.9.2342.19200300.100.1.1.html (submitted but not verified OIDs)
        // http://oid-info.com/get/1.2.840.113549.1.1.10
        // See also http://www.itu.int/en/ITU-T/asn1/Pages/OID-project.aspx

        public OidInfoControl()
        {
            InitializeComponent();

            inputBox.TextChanged += (s, _) => searchButton.Enabled = !string.IsNullOrEmpty(inputBox.Text);
            searchButton.Click += (s, _) => Search(inputBox.Text);
            clearButton.Click += (s, _) => logbox.Clear();

            // Initialize with an sample OID
            inputBox.Text = "1.2.840.113549.1.1.10";
        }

        private void Search(string oidText)
        {
            try
            {
                var oid = new Oid(oidText);
                LogLn(string.Format("OID {0} is {1}", oidText, oid.FriendlyName));
            }
            catch (Exception ex)
            {
                LogLn("ERROR: " + ex.Message);
            }            
        }

        private void LogLn(string text = "") { Log(text + "\r\n"); }

        /// <summary>
        /// Logs the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void Log(string text)
        {
            logbox.AppendText(text);
            logbox.Select(logbox.Text.Length, 0);
        }
    }
}
