namespace Delta.CapiNet.Asn1
{
    // According to PKCS, octet strings may be structures...
    // This is why we try to find children in it.
    public class Asn1OctetString : Asn1StructuredObject
    {
        public Asn1OctetString(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { } 

        protected override void ParseContent()
        {
            if (Document != null && Document.ParseOctetStrings)
                base.ParseContent();
            else Nodes = new Asn1Object[0];
        }

        public override string ToString() => $"OctetString: {Workload.ToFormattedString()}";
    }
}
