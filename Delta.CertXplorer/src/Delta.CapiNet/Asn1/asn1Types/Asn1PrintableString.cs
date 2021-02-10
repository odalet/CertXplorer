using System.Text;

namespace Delta.CapiNet.Asn1
{
    internal class Asn1PrintableString : BaseAsn1String
    {
        private static readonly Encoding windows1252 = Encoding.GetEncoding(1252);

        public Asn1PrintableString(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        protected override string FriendlyName => "PrintableString";
        protected override Encoding Encoding => windows1252;
    }
}
