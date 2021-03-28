using System;
using Microsoft.Win32.SafeHandles;

namespace Delta.CapiNet.Internals
{
    /// <summary>
    /// <b>Safely</b> holds a local memory allocation handle.
    /// </summary>
    internal sealed class LocalAllocHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public LocalAllocHandle(IntPtr handle) : base(true) => SetHandle(handle);

        public static LocalAllocHandle InvalidHandle => new(IntPtr.Zero);

        /// <summary>
        /// Allocates memory using the Win32 <c>LocalAlloc</c> API function and safely stores the returned handle.
        /// </summary>
        /// <param name="uFlags">The allocation flags.</param>
        /// <param name="sizetdwBytes">The size to allocate in bytes.</param>
        /// <returns>An instance of <see cref="LocalAllocHandle"/> or an <see cref="OutOfMemoryException"/> 
        /// if allocation failed.</returns>
        /// <remarks>
        /// See <c>LocalAlloc</c> documentation here for more information: 
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa366723.aspx
        /// </remarks>
        public static LocalAllocHandle Allocate(uint uFlags, IntPtr size)
        {
            var handle = NativeMethods.LocalAlloc(uFlags, size);
            return handle == null || handle.IsInvalid ?
                throw new OutOfMemoryException("Could not allocate memory using LocalAlloc native call.") :
                handle;
        }

        protected override bool ReleaseHandle() => NativeMethods.LocalFree(handle) == IntPtr.Zero;
    }
}
