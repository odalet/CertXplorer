using Delta.CapiNet.Asn1;

namespace Delta.Icao.Asn1
{
    public abstract class Asn1DataGroupData : Asn1StructuredObject
    {
        protected Asn1DataGroupData(Asn1Document document, TaggedObject content, Asn1Object parentObject) :
            base(document, content, parentObject) => EFTag = (Tags.EF)content.Tag.Value;

        public Tags.EF EFTag { get; }

        public virtual string Name => EFTag.ToString();

        /// <inheritdoc/>
        public override string ToString() => $"EF.{Name}";
    }
}
