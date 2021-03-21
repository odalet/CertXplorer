namespace Delta.CapiNet.Asn1
{
    public class Asn1NumericString : Asn1Object
    {
        public Asn1NumericString(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        public byte[] Value => Workload;

        public override string ToString() => $"Asn1NumericString: {Value.ToFormattedString()}";
    }
}
