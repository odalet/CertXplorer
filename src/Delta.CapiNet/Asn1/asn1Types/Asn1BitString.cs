using System.Linq;

namespace Delta.CapiNet.Asn1
{
    public class Asn1BitString : Asn1Object
    {
        public Asn1BitString(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject)
        {
            UnusedBytes = content.Workload[0];
            BitString = content.Workload.Skip(1).ToArray();
        }

        public byte UnusedBytes { get; }
        public byte[] BitString { get; }

        public override string ToString() => $"BitString ({UnusedBytes} padding bytes):\r\n{BitString.ToFormattedString()}";
    }
}
