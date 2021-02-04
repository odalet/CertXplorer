using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using Delta.CapiNet;

namespace Delta.CertXplorer.DocumentModel
{
    /// <summary>
    /// Wraps either a X509 certificate, a CRL or a CTL (certificate trust list).
    /// </summary>
    internal sealed class X509Object
    {
        private X509Object(string storeName, StoreLocation storeLocation)
        {
            StoreName = storeName;
            StoreLocation = storeLocation;
        }

        public string StoreName { get; }
        public StoreLocation StoreLocation { get; }
        public object Value { get; private set; }
        public byte[] Data { get; private set; }
        public string DisplayName => FormatDisplayName(StoreName, StoreLocation);

        public static X509Object Create(X509Certificate2 certificate, string storeName, StoreLocation storeLocation)
        {
            if (certificate == null) throw new ArgumentNullException("certificate");

            return new X509Object(storeName, storeLocation)
            {
                Value = certificate,
                Data = certificate.RawData
            };
        }

        public static X509Object Create(CertificateRevocationList crl, string storeName, StoreLocation storeLocation)
        {
            if (crl == null) throw new ArgumentNullException("crl");
            return new X509Object(storeName, storeLocation)
            {
                Value = crl,
                Data = crl.RawData
            };
        }

        public static X509Object Create(CertificateTrustList ctl, string storeName, StoreLocation storeLocation)
        {
            if (ctl == null) throw new ArgumentNullException("ctl");
            return new X509Object(storeName, storeLocation)
            {
                Value = ctl,
                Data = ctl.RawData
            };
        }

        /// <summary>
        /// Formats the display name.
        /// </summary>
        /// <param name="storeName">Name of the certififcates store.</param>
        /// <param name="storeLocation">The store location.</param>
        private string FormatDisplayName(string storeName, StoreLocation storeLocation)
        {
            var fullStoreName = $"{storeLocation}/{storeName}";
            var certName = "?";

            if (Value == null) certName = "Null";
            if (Value is X509Certificate2 xc2) certName = GetX509CertificateDisplayName(xc2);
            if (Value is CertificateRevocationList crl) certName = GetCrlDisplayName(crl);
            if (Value is CertificateTrustList ctl) certName = GetCtlDisplayName(ctl);

            return $"{fullStoreName}/{certName}";
        }

        private string GetCrlDisplayName(CertificateRevocationList crl) =>
            string.IsNullOrEmpty(crl.FriendlyName) ? FormatDistinguishedName(crl.IssuerName) : crl.FriendlyName;

        private string GetCtlDisplayName(CertificateTrustList ctl) =>
            string.IsNullOrEmpty(ctl.FriendlyName) ? "Trust List" : ctl.FriendlyName;

        private string GetX509CertificateDisplayName(X509Certificate2 x509) =>
            string.IsNullOrEmpty(x509.FriendlyName) ? FormatDistinguishedName(x509.SubjectName) : x509.FriendlyName;

        [SuppressMessage("Minor Code Smell", "S1643:Strings should not be concatenated using '+' in a loop", Justification = "Acceptable here")]
        private string FormatDistinguishedName(X500DistinguishedName dn)
        {
            var subjectName = dn.Name;
            if (subjectName.Contains("\""))
            {
                var insideQuotes = false;
                var subjectName2 = string.Empty;
                for (var i = 0; i < subjectName.Length; i++)
                {
                    if (subjectName[i] == '"') insideQuotes = !insideQuotes;
                    if (subjectName[i] == ',' && insideQuotes)
                        subjectName2 += '#';
                    else subjectName2 += subjectName[i];
                }

                subjectName = subjectName2;
            }

            var parts = subjectName.Split(',');

            var part = parts.Length == 0 ? dn.Name : parts[0];
            part = part.Replace('#', ',');
            part = part.Replace("\"", string.Empty);

            var index = part.IndexOf('=');
            if (index != -1) part = part.Substring(index + 1);

            return part;
        }
    }
}
