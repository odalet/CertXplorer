using Delta.CapiNet.Asn1;

namespace Delta.Icao.Asn1
{
    public abstract class Asn1DataGroupData : Asn1StructuredObject
    {
        public Asn1DataGroupData(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) 
        {
            EFTag = (Tags.EF)content.Tag.Value;
        }
        
        public Tags.EF EFTag
        {
            get; private set;
        }

        public virtual string Name 
        {
            get { return EFTag.ToString(); }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("EF.{0}", Name);
        }
    }
}
