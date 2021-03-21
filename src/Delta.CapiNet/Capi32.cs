using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Delta.CapiNet.Internals;

namespace Delta.CapiNet
{
    public static class Capi32
    {
        private sealed class EnumSystemStore
        {
            private readonly uint flags;
            private List<string> tempSystemStoreList;

            public EnumSystemStore(uint locationFlags) => flags = locationFlags;

            public string[] GetStores()
            {
                tempSystemStoreList = new List<string>();
                var cb = new NativeMethods.CertEnumSystemStoreCallback(CertEnumSystemStoreCallback);
                var ok = NativeMethods.CertEnumSystemStore(flags, 0, IntPtr.Zero, cb);

                var array = ok ? tempSystemStoreList.ToArray() : (new string[] { });
                tempSystemStoreList = null;
                return array;
            }

            private bool CertEnumSystemStoreCallback(string pvSystemStore, uint dwFlags, ref CERT_SYSTEM_STORE_INFO pStoreInfo, uint pvReserved, IntPtr pvArg)
            {
                tempSystemStoreList.Add(pvSystemStore);
                return true;
            }
        }

        private sealed class EnumPhysicalStore
        {
            private readonly string systemStoreName;
            private List<string> tempPhysicalStoreList;

            public EnumPhysicalStore(string parent) => systemStoreName = parent;

            public string[] GetStores()
            {
                tempPhysicalStoreList = new List<string>();
                var cb = new NativeMethods.CertEnumPhysicalStoreCallback(CertEnumPhysicalStoreCallback);
                var ok = NativeMethods.CertEnumPhysicalStore(systemStoreName, CapiConstants.CERT_SYSTEM_STORE_CURRENT_USER, IntPtr.Zero, cb);

                var array = ok ? tempPhysicalStoreList.ToArray() : (new string[] { });
                tempPhysicalStoreList = null;
                return array;
            }

            private bool CertEnumPhysicalStoreCallback(string pvSystemStore, uint dwFlags, string pwszStoreName, ref CERT_PHYSICAL_STORE_INFO pStoreInfo, uint pvReserved, IntPtr pvArg)
            {
                tempPhysicalStoreList.Add(pwszStoreName);
                return true;
            }
        }

        public static IEnumerable<string> GetPhysicalStores() => GetPhysicalStores("My");
        public static IEnumerable<string> GetPhysicalStores(string systemStoreName) => 
            new EnumPhysicalStore(systemStoreName).GetStores();

        public static IEnumerable<CertificateStore> GetSystemStores()        
        {
            var names = new EnumSystemStore(CapiConstants.CERT_SYSTEM_STORE_CURRENT_USER).GetStores();
            return names.Select(name => new CertificateStore(name, CertificateStoreLocation.FromId(CapiConstants.CERT_SYSTEM_STORE_CURRENT_USER_ID)));
        }

        public static IEnumerable<CertificateStore> GetSystemStores(StoreLocation storeLocation) => 
            GetSystemStores(CertificateStoreLocation.FromStoreLocation(storeLocation));

        public static IEnumerable<CertificateStore> GetSystemStores(CertificateStoreLocation systemStoreLocation)
        {
            var names = new EnumSystemStore(systemStoreLocation.Flags).GetStores();
            return names.Select(name => new CertificateStore(name, systemStoreLocation));
        }

        public static CertificateStore GetCertificateStore(string storeName, StoreLocation storeLocation) => 
            GetCertificateStore(storeName, CertificateStoreLocation.FromStoreLocation(storeLocation));

        public static CertificateStore GetCertificateStore(string storeName, CertificateStoreLocation systemStoreLocation) => 
            new(storeName, systemStoreLocation);

        public static string LocalizeName(string name) => NativeMethods.CryptFindLocalizedName(name);
    }
}
