using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Delta.CapiNet;

namespace Delta.CertXplorer.CertManager
{
    partial class CertificateListControl
    {
        // Merges Certificates, Crls and Ctls into a structure
        // easily convertible to a ListViewItem
        private sealed class X509Item
        {
            public X509Item(Certificate certificate)
            {
                Tag = certificate;
                IssuedTo = FormatDN(certificate.SubjectName);
                IssuedBy = FormatDN(certificate.IssuerName);
                From = FormatDate(certificate.X509.NotBefore);
                To = FormatDate(certificate.X509.NotAfter);
                FriendlyName = certificate.FriendlyName;
                IsValid = certificate.IsValid;
                HasPrivateKey = certificate.HasPrivateKey;
                AllFields = MakeFieldsCache();
            }

            public X509Item(CertificateRevocationList crl)
            {
                Tag = crl;
                IssuedTo = "Revocation List";
                IssuedBy = FormatDN(crl.IssuerName);
                From = FormatDate(crl.PublicationDate);
                To = FormatDate(crl.NextUpdate);
                FriendlyName = crl.FriendlyName;
                IsValid = crl.IsValid;
                AllFields = MakeFieldsCache();
            }

            public X509Item(CertificateTrustList ctl)
            {
                Tag = ctl;
                IssuedTo = "Trust List";
                IssuedBy = string.Empty;
                From = FormatDate(ctl.PublicationDate);
                To = FormatDate(ctl.NextUpdate);
                FriendlyName = ctl.FriendlyName;
                IsValid = ctl.IsValid;
                AllFields = MakeFieldsCache();
            }

            public string IssuedTo { get; }
            public string IssuedBy { get; }
            public string From { get; }
            public string To { get; }
            public string FriendlyName { get; }
            public bool IsValid { get; }
            public bool HasPrivateKey { get; }
            public object Tag { get; }
            private string[] AllFields { get; }

            public bool Match(string filter)
            {
                var upper = filter.ToUpperInvariant();
                return AllFields.Any(x => x.Contains(upper));
            }

            private string[] MakeFieldsCache() => new[]
            {
                IssuedTo.ToUpperInvariant(),
                IssuedBy.ToUpperInvariant(),
                From.ToUpperInvariant(),
                To.ToUpperInvariant(),
                FriendlyName.ToUpperInvariant()
            };

            private static string FormatDN(X500DistinguishedName dn)
            {
                var cn = dn.ExtractRdn("cn");
                if (!string.IsNullOrEmpty(cn)) return cn;

                var ou = dn.ExtractRdn("ou");
                if (!string.IsNullOrEmpty(ou)) return ou;

                return dn.Name;
            }

            private static string FormatDate(DateTime date) => 
                date == DateTime.MinValue ? string.Empty : date.ToString("yyyy/MM/dd");

            private static string FormatDate(DateTimeOffset date) => 
                date == DateTimeOffset.MinValue ? string.Empty : date.ToString("yyyy/MM/dd");
        }
    }
}
