using System;
using System.Security.Cryptography.X509Certificates;
using Delta.CapiNet.Internals;

namespace Delta.CapiNet
{
    public static class UI
    {
        public static void ShowCertificateDialog(this X509Certificate2 certificate) => ShowCertificateDialog(certificate, IntPtr.Zero);
        public static void ShowCertificateDialog(this X509Certificate2 certificate, IntPtr owner) =>
            ShowCertDialog(certificate, owner, null);

        public static void ShowCertificateDialog(this Certificate certificate) => ShowCertificateDialog(certificate, IntPtr.Zero);
        public static void ShowCertificateDialog(this Certificate certificate, IntPtr owner) =>
            ShowCertDialog(certificate.X509, owner, null);

        private static void ShowCertDialog(X509Certificate cert, IntPtr owner, string title) => NativeMethods.CryptUIDlgViewContext(
            CapiConstants.CERT_STORE_CERTIFICATE_CONTEXT, cert.Handle, owner, title, 0, IntPtr.Zero);

        public static void ShowCrlDialog(this CertificateRevocationList crl) => ShowCrlDialog(crl, IntPtr.Zero, null);
        public static void ShowCrlDialog(this CertificateRevocationList crl, string title) => ShowCrlDialog(crl, IntPtr.Zero, title);
        public static void ShowCrlDialog(this CertificateRevocationList crl, IntPtr owner) => ShowCrlDialog(crl, owner, null);
        public static void ShowCrlDialog(this CertificateRevocationList crl, IntPtr owner, string title) => NativeMethods.CryptUIDlgViewContext(
            CapiConstants.CERT_STORE_CRL_CONTEXT, crl.SafeHandle, owner, title, 0, IntPtr.Zero);

        public static void ShowCtlDialog(this CertificateTrustList ctl) => ShowCtlDialog(ctl, IntPtr.Zero, null);
        public static void ShowCtlDialog(this CertificateTrustList ctl, string title) => ShowCtlDialog(ctl, IntPtr.Zero, title);
        public static void ShowCtlDialog(this CertificateTrustList ctl, IntPtr owner) => ShowCtlDialog(ctl, owner, null);
        public static void ShowCtlDialog(this CertificateTrustList ctl, IntPtr owner, string title) => NativeMethods.CryptUIDlgViewContext(
            CapiConstants.CERT_STORE_CTL_CONTEXT, ctl.SafeHandle, owner, title, 0, IntPtr.Zero);

        public static void ShowCertificatesDialog() => ShowCertificatesDialog(IntPtr.Zero);
        public static void ShowCertificatesDialog(IntPtr owner) => NativeMethods.OpenPersonalTrustDBDialog(owner);

        public static void ShowTrustedPublishersDialog() => ShowTrustedPublishersDialog(IntPtr.Zero);

        public static void ShowTrustedPublishersDialog(IntPtr owner) => NativeMethods.OpenPersonalTrustDBDialogEx(
            owner, CapiConstants.WT_TRUSTDBDIALOG_ONLY_PUB_TAB_FLAG, IntPtr.Zero);

        public static void ShowBuildCtlWizard() => ShowBuildCtlWizard(IntPtr.Zero, null);
        public static void ShowBuildCtlWizard(string title) => ShowBuildCtlWizard(IntPtr.Zero, title);
        public static void ShowBuildCtlWizard(IntPtr owner) => ShowBuildCtlWizard(owner, null);
        public static void ShowBuildCtlWizard(IntPtr owner, string title) => NativeMethods.CryptUIWizBuildCTL(
            0, owner, title, IntPtr.Zero, IntPtr.Zero, out _);
    }
}
