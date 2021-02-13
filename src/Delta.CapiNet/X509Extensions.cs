using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Delta.CapiNet.Internals;
using Delta.CapiNet.Logging;
using COM = System.Runtime.InteropServices.ComTypes;

namespace Delta.CapiNet
{
    /// <summary>
    /// Extension methods defined on various X509 related classes.
    /// </summary>
    public static class X509Extensions
    {
        private static readonly ILogService log = LogManager.GetLogger(typeof(X509Extensions));

        /// <summary>
        /// Gets the certificate revocation lists contained in the specified store.
        /// </summary>
        /// <param name="store">The X509 Certificates store.</param>
        /// <returns>A collection of <see cref="CertificateRevocationList"/> objects.</returns>
        public static IEnumerable<CertificateRevocationList> GetCertificateRevocationLists(this X509Store store)
        {
            var handle = CertStoreHandle.FromX509Store(store);
            if (handle.IsInvalid || handle.IsClosed)
                return new CertificateRevocationList[0];  // Empty list

            var list = new List<CertificateRevocationList>();
            for (
                    var ptr = NativeMethods.CertEnumCRLsInStore(handle, IntPtr.Zero);
                    ptr != IntPtr.Zero;
                    ptr = NativeMethods.CertEnumCRLsInStore(handle, ptr)
                ) list.Add(new CertificateRevocationList(ptr));

            return list; // Don't free the handle: it is linked to the store object.
        }

        /// <summary>
        /// Gets the certificate trust lists contained in the specified store.
        /// </summary>
        /// <param name="store">The X509 Certificates store.</param>
        /// <returns>A collection of <see cref="CertificateTrustList"/> objects.</returns>
        public static IEnumerable<CertificateTrustList> GetCertificateTrustLists(this X509Store store)
        {
            var handle = CertStoreHandle.FromX509Store(store);
            if (handle.IsInvalid || handle.IsClosed)
                return new CertificateTrustList[0];  // Empty list

            var list = new List<CertificateTrustList>();
            for (
                    var ptr = NativeMethods.CertEnumCTLsInStore(handle, IntPtr.Zero);
                    ptr != IntPtr.Zero;
                    ptr = NativeMethods.CertEnumCTLsInStore(handle, ptr)
                ) list.Add(new CertificateTrustList(ptr));

            return list; // Don't free the handle: it is linked to the store object.
        }

        /// <summary>
        /// Gets the certificates contained in the specified store.
        /// </summary>
        /// <param name="store">The X509 Certificates store.</param>
        /// <returns>A collection of <see cref="Certificate"/> objects.</returns>
        public static IEnumerable<Certificate> GetCertificates(this X509Store store) => 
            store.Certificates.Cast<X509Certificate2>().Select(x509 => new Certificate(x509));

        /// <summary>
        /// Extracts the specified relative distinguished name from an X500 Distinguished Name.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>If the specified rdn appears more than once, only the first rdn is retrieved.</item>
        /// <item>
        /// if the specified rdn's value is enclosed in quotes (because of a comma in its 
        /// value for example), the quotes are not returned.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="distinguishedName">The X500 Distinguished Name.</param>
        /// <param name="rdn">The relative distinguished name.</param>
        /// <returns>The value of the specified relative distinguished name.</returns>
        public static string ExtractRdn(this X500DistinguishedName distinguishedName, string rdn) => ExtractRdn(distinguishedName.Name, rdn);

        /// <summary>
        /// Extracts the specified relative distinguished name from an X500 Distinguished Name.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>If the specified rdn appears more than once, only the first rdn is retrieved.</item>
        /// <item>
        /// if the specified rdn's value is enclosed in quotes (because of a comma in its 
        /// value for example), the quotes are not returned.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="distinguishedName">The X500 Distinguished Name.</param>
        /// <param name="rdn">The relative distinguished name.</param>
        /// <returns>The value of the specified relative distinguished name.</returns>
        public static string ExtractRdn(this string distinguishedName, string rdn)
        {
            var len = distinguishedName.Length;

            // rdn normalization.
            if (!rdn.EndsWith("=")) rdn += "=";
            rdn = rdn.ToUpperInvariant();

            var rdnStart = distinguishedName.IndexOf(rdn);
            if (rdnStart == -1) return string.Empty;

            var start = rdnStart + rdn.Length;
            if (start >= len) return string.Empty;

            var charToFind = ',';
            if (distinguishedName[start] == '"')
            {
                charToFind = '"';
                start++;
            }

            if (start >= len) return string.Empty;

            var end = distinguishedName.IndexOf(charToFind, start);
            return end == -1 ?
                distinguishedName.Substring(start) :
                distinguishedName.Substring(start, end - start);
        }

        internal static byte[] ToByteArray(this CRYPTOAPI_BLOB blob)
        {
            if (blob.cbData == 0) return new byte[0];

            var destination = new byte[blob.cbData];
            Marshal.Copy(blob.pbData, destination, 0, destination.Length);
            return destination;
        }

        internal static DateTimeOffset ToDateTimeOffset(this COM.FILETIME fileTime)
        {
            try
            {
                var hi = (ulong)fileTime.dwHighDateTime;
                var lo = (ulong)fileTime.dwLowDateTime;
                var value = (long)((hi << 0x20) | lo);
                return DateTimeOffset.FromFileTime(value);
            }
            catch (Exception ex)
            {
                log.Error($"Conversion to {nameof(ToDateTimeOffset)} of {nameof(COM.FILETIME)} object failed: {ex.Message}", ex);
                return DateTimeOffset.MinValue; // Means invalid.
            }
        }
    }
}
