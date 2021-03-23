using System;

namespace Delta.CapiNet.Asn1
{
    public sealed class Asn1Oid : Asn1Object
    {
        public Asn1Oid(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) => Value = OidUtils.Decode(content.Workload);

        public string Value { get; }

        public string Name 
        {
            get 
            {
                var name = OidUtils.GetOidName(Value);
                if (string.IsNullOrEmpty(name)) name = "Unknown";
                return name;
            }
        }

        public override string ToString() => $"Oid: {Name} ({Value})";
    }
}
