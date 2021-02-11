namespace Delta.CapiNet.Asn1
{
    public sealed class Asn1Utf8String : BaseAsn1String
    {
        public Asn1Utf8String(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        protected override string FriendlyName => "Utf8String";
    }
}
