using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Delta.CapiNet.Internals;

namespace Delta.CapiNet
{
    /// <summary>
    /// Represents Locations of Certificate Stores.
    /// </summary>
    public class CertificateStoreLocation
    {
        private readonly struct SystemStoreLocationInfo
        {
            public SystemStoreLocationInfo(string name, uint flags)
            {
                Name = name;
                Flags = flags;
            }

            public string Name { get; }
            public uint Flags { get; }            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateStoreLocation"/> class.
        /// </summary>
        /// <param name="systemStoreLocationInfo">The system store location info.</param>
        private CertificateStoreLocation(SystemStoreLocationInfo systemStoreLocationInfo)
        {
            Name = systemStoreLocationInfo.Name;
            Flags = systemStoreLocationInfo.Flags;
            Id = FlagsToId(Flags);
        }
        
        public string Name { get; }
        public uint Id { get; }
        internal uint Flags { get; }

        public StoreLocation ToStoreLocation() => Enum.IsDefined(typeof(StoreLocation), (int)Id) ?
            (StoreLocation)Enum.ToObject(typeof(StoreLocation), (int)Id) :
            throw new InvalidOperationException($"This instance's current value can't be represented as a {nameof(StoreLocation)}.");

        public override string ToString() => string.IsNullOrEmpty(Name) ? "?" : Name;

        public static IEnumerable<CertificateStoreLocation> GetSystemStoreLocations()
        {
            var locations = new List<SystemStoreLocationInfo>();
            var ok = NativeMethods.CertEnumSystemStoreLocation(0, IntPtr.Zero, (name, flags, _, _) =>
            {
                locations.Add(new SystemStoreLocationInfo(name, flags));
                return true;
            });

            return ok ?
                locations.Select(info => new CertificateStoreLocation(info)) :
                new CertificateStoreLocation[0];
        }

        public static CertificateStoreLocation FromStoreLocation(StoreLocation location) => new CertificateStoreLocation(
            new SystemStoreLocationInfo(location.ToString(), IdToFlags((uint)location)));

        internal static CertificateStoreLocation FromId(uint id) => new CertificateStoreLocation(
            new SystemStoreLocationInfo(id.ToString(), IdToFlags(id)));

        private static uint IdToFlags(uint id) => id << (int)CapiConstants.CERT_SYSTEM_STORE_LOCATION_SHIFT;
        private static uint FlagsToId(uint flags) => flags >> (int)CapiConstants.CERT_SYSTEM_STORE_LOCATION_SHIFT;
    }
}
