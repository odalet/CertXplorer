using System.Security.Cryptography.X509Certificates;

using Delta.CapiNet.Internals;

namespace Delta.CapiNet
{
    public sealed class CertificateStore
    {
        internal CertificateStore(string storeName, CertificateStoreLocation storeLocation)
        {
            Name = storeName;
            Location = storeLocation;
        }

        public string Name { get; }
        public CertificateStoreLocation Location { get; }

        public string LocalizedName => NativeMethods.CryptFindLocalizedName(Name);

        public X509Store GetX509Store() => new X509Store(Name, Location.ToStoreLocation());

        public string ToLongString()
        {
            var localizedName = LocalizedName;
            return string.IsNullOrEmpty(localizedName) ? 
                ToString() : 
                $"{ToString()} [{localizedName}]";
        }

        public override string ToString() => string.IsNullOrEmpty(Name) ? "?" : Name;
    }
}
