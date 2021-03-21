namespace Delta.CapiNet.Asn1
{
    public class Asn1ContextSpecific : Asn1StructuredObject
    {
        internal Asn1ContextSpecific(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        public override string ToString() => $"ContextSpecific: Tag = 0x{TaggedObject.Tag.Value:X2}";
    }
}
