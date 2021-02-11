using System.ComponentModel;
using System.Security.Cryptography;

namespace Delta.CertXplorer.CertManager.Wrappers
{
    [TypeConverter(typeof(CustomExpandableObjectConverter))]
    internal sealed class OidWrapper : BaseWrapper, IDisplayTypeWrapper
    {
        private readonly Oid oid;

        public OidWrapper(Oid o) => oid = o;

        public string FriendlyName => TryGet(() => oid.FriendlyName);
        public string Value => TryGet(() => oid.Value);
        public string DisplayType
        {
            get
            {
                if (oid == null) return "NULL";
                if (string.IsNullOrEmpty(oid.FriendlyName))
                    return string.IsNullOrEmpty(oid.Value) ? string.Empty : oid.Value;

                return string.IsNullOrEmpty(oid.Value) ? oid.FriendlyName :
                    string.Format("{0} ({1})", oid.FriendlyName, oid.Value);
            }
        }
    }
}
