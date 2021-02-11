using System.Security.Cryptography.X509Certificates;
using Delta.CapiNet;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    internal static class ObjectWrapper
    {
        /// <summary>
        /// Wraps the specified item so that it is displayed friendly in a property grid.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Wrapped item.</returns>
        public static object Wrap(object item)
        {
            if (item == null) return null;
            if (item is Certificate c) return new CapiCertificateWrapper(c);
            if (item is CertificateRevocationList crl) return new CapiCrlWrapper(crl);
            if (item is CertificateTrustList ctl) return new CapiCtlWrapper(ctl);
            if (item is X509Certificate2 xc2) return new X509CertificateWrapper2(xc2);
            if (item is X509Certificate xc) return new X509CertificateWrapper(xc);
            return item;
        }
    }
}
