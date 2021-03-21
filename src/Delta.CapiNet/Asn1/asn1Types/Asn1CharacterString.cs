namespace Delta.CapiNet.Asn1
{
    internal class Asn1CharacterString : BaseAsn1String
    {
        public Asn1CharacterString(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        protected override string FriendlyName => "CharacterString";
    }
}
