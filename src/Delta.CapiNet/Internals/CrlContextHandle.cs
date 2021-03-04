using System;

using Microsoft.Win32.SafeHandles;

namespace Delta.CapiNet.Internals
{
    /// <summary>
    /// <b>Safely</b> holds a CRL (Certificate Revocation List) context handle.
    /// </summary>
    internal sealed class CrlContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private CrlContextHandle() : base(true) { }

        public CrlContextHandle(IntPtr handle) : base(true) => SetHandle(handle);

        public static CrlContextHandle InvalidHandle => new CrlContextHandle(IntPtr.Zero);

        protected override bool ReleaseHandle() => NativeMethods.CertFreeCRLContext(base.handle);
    }
}
