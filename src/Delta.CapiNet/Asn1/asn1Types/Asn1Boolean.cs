namespace Delta.CapiNet.Asn1
{
    public class Asn1Boolean : Asn1Object
    {
        public Asn1Boolean(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) => Value = content.Workload[0] == 0xFF;

        public bool Value { get; }

        public override string ToString() => $"Boolean: {Value}";
    }
}
