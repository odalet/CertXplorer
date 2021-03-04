using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Delta.CapiNet.Internals;

namespace Delta.CapiNet
{
    [SuppressMessage("Blocker Bug", "S3869:\"SafeHandle.DangerousGetHandle\" should not be called", Justification = "Required here: unsafe code")]
    public sealed class CertificateRevocationList
    {        
        public CertificateRevocationList(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException("Invalid Handle");
            
            SafeHandle = NativeMethods.CertDuplicateCRLContext(handle);
            if (SafeHandle.IsInvalid) throw new CryptographicException("Invalid Handle");

            CrlInfo = GetCrlInfo();
            RawData = GetRawData();
        }

        public string FriendlyName
        {
            get
            {
                var allocHandle = LocalAllocHandle.InvalidHandle;
                uint pcbData = 0;

                // 1st call gives the memory amount to allocate
                if (!NativeMethods.CertGetCRLContextProperty(
                    SafeHandle, CapiConstants.CERT_FRIENDLY_NAME_PROP_ID, allocHandle, ref pcbData)) 
                    return string.Empty;

                // 2nd call fills the memory
                allocHandle = LocalAllocHandle.Allocate(0, new IntPtr((long)pcbData));
                try
                {
                    if (!NativeMethods.CertGetCRLContextProperty(
                        SafeHandle, CapiConstants.CERT_FRIENDLY_NAME_PROP_ID, allocHandle, ref pcbData))
                        return string.Empty;

                    // Get the data back to a managed string and release the native memory.
                    var result = Marshal.PtrToStringUni(allocHandle.DangerousGetHandle());
                    return result;
                }
                finally { allocHandle.Dispose(); }
            }
        }

        public X500DistinguishedName IssuerName => CrlInfo.HasValue ? new X500DistinguishedName(CrlInfo.Value.Issuer.ToByteArray()) : null;
        public DateTimeOffset PublicationDate => CrlInfo.HasValue ? CrlInfo.Value.ThisUpdate.ToDateTimeOffset() : DateTimeOffset.MinValue;
        public DateTimeOffset NextUpdate => CrlInfo.HasValue ? CrlInfo.Value.NextUpdate.ToDateTimeOffset() : DateTimeOffset.MinValue;

        public bool IsValid
        {
            get
            {
                var publicationDate = PublicationDate;
                if (publicationDate == DateTimeOffset.MinValue) return false;
                
                var nextUpdate = NextUpdate;
                if (nextUpdate == DateTimeOffset.MinValue) return false;
                
                var now = DateTimeOffset.Now;
                return now >= publicationDate && now <= nextUpdate;
            }
        }

        public byte[] RawData { get; }

        internal CrlContextHandle SafeHandle { get; }
        private CRL_INFO? CrlInfo { get; }

        [SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "For debugging purpose")]
        private unsafe CRL_INFO? GetCrlInfo()
        {
            try
            {
                var crlContext = *(CRL_CONTEXT*)SafeHandle.DangerousGetHandle();
                return (CRL_INFO)Marshal.PtrToStructure(crlContext.pCrlInfo, typeof(CRL_INFO));
            }
            catch (Exception ex)
            {
                var debugEx = ex; // for debugging purpose
            }

            return null;
        }

        [SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "For debugging purpose")]
        private unsafe byte[] GetRawData()
        {
            try
            {
                var crlContext = *(CRL_CONTEXT*)SafeHandle.DangerousGetHandle();
                var size = (int)crlContext.cbCrlEncoded;
                var data = new byte[size];
                Marshal.Copy(crlContext.pbCrlEncoded, data, 0, size);
                return data;
            }
            catch (Exception ex)
            {
                var debugEx = ex; // for debugging purpose
            }

            return new byte[0];
        }
    }
}
