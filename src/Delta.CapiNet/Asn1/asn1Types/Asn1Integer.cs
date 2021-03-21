namespace Delta.CapiNet.Asn1
{
    public class Asn1Integer : Asn1Object
    {
        public Asn1Integer(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject)
        {
            var value = 0L;
            foreach (var element in content.Workload)
            {
                value <<= 8;
                value += element;
            }

            Value = value;
        }

        public long Value { get; }

        public override string ToString() => $"Integer: {Value} (0x{Value:X})";
    }
}
