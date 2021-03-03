using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Delta.CapiNet.Internals;
using Delta.CapiNet.Logging;

namespace Delta.CapiNet
{
    [SuppressMessage("Blocker Bug", "S3869:\"SafeHandle.DangerousGetHandle\" should not be called", Justification = "By design")]
    public sealed class CertificateTrustList
    {
        private static readonly ILogService log = LogManager.GetLogger<CertificateTrustList>();

        public CertificateTrustList(IntPtr handle)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException("Invalid Handle");
            SafeHandle = NativeMethods.CertDuplicateCTLContext(handle);

            // Retrieve properties
            FriendlyName = GetFriendlyName(SafeHandle);
            CtlInfo = GetCtlInfo(SafeHandle);
            RawData = GetRawData(SafeHandle);
        }

        public string FriendlyName { get; }
        public byte[] RawData { get; }
        public DateTimeOffset PublicationDate => CtlInfo.HasValue ? CtlInfo.Value.ThisUpdate.ToDateTimeOffset() : DateTimeOffset.MinValue;
        public DateTimeOffset NextUpdate => CtlInfo.HasValue ? CtlInfo.Value.NextUpdate.ToDateTimeOffset() : DateTimeOffset.MinValue;

        public bool IsValid
        {
            get
            {
                var now = DateTime.Now;
                return now >= PublicationDate && now <= NextUpdate;
            }
        }

        internal CtlContextHandle SafeHandle { get; }
        private CTL_INFO? CtlInfo { get; }

        private static string GetFriendlyName(CtlContextHandle handle)
        {
            if (handle.IsInvalid) throw new CryptographicException("Invalid Handle");

            var allocHandle = LocalAllocHandle.InvalidHandle;
            uint pcbData = 0;

            // 1st call gives the memory amount to allocate
            if (!NativeMethods.CertGetCTLContextProperty(
                handle, CapiConstants.CERT_FRIENDLY_NAME_PROP_ID, allocHandle, ref pcbData))
                return string.Empty;

            // 2nd call fills the memory
            allocHandle = LocalAllocHandle.Allocate(0, new IntPtr((long)pcbData));
            try
            {
                if (!NativeMethods.CertGetCTLContextProperty(
                    handle, CapiConstants.CERT_FRIENDLY_NAME_PROP_ID, allocHandle, ref pcbData))
                    return string.Empty;

                // Get the data back to a managed string and release the native memory.
                var result = Marshal.PtrToStringUni(allocHandle.DangerousGetHandle());
                return result;
            }
            finally { allocHandle.Dispose(); }
        }

        private static unsafe byte[] GetRawData(CtlContextHandle handle)
        {
            if (handle.IsInvalid) throw new CryptographicException("Invalid Handle");
            try
            {
                var ctlContext = *(CTL_CONTEXT*)handle.DangerousGetHandle();
                var size = (int)ctlContext.cbCtlEncoded;
                var data = new byte[size];
                Marshal.Copy(ctlContext.pbCtlEncoded, data, 0, size);
                return data;
            }
            catch (Exception ex)
            {
                log.Error($"Could not retrieve CTL Raw Data: {ex.Message}", ex);
            }

            return new byte[0];
        }

        private static unsafe CTL_INFO? GetCtlInfo(CtlContextHandle handle)
        {
            if (handle.IsInvalid) throw new CryptographicException("Invalid Handle");
            try
            {
                var ctlContext = *(CTL_CONTEXT*)handle.DangerousGetHandle();
                return (CTL_INFO)Marshal.PtrToStructure(ctlContext.pCtlInfo, typeof(CTL_INFO));
            }
            catch (Exception ex)
            {
                log.Error($"Could not retrieve CTL Info: {ex.Message}", ex);
            }

            return null;
        }
    }
}
