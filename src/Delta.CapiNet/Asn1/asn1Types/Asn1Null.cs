namespace Delta.CapiNet.Asn1
{
    public class Asn1Null : Asn1Object
    {
        private readonly bool warning;

        public Asn1Null(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) => warning = content.WorkloadLength != 0;

        public override string ToString() => warning ? "Null: WARNING - Should be empty." : "Null";
    }
}
