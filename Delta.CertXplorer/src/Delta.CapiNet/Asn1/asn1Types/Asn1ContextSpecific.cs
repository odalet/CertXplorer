using System;

namespace Delta.CapiNet.Asn1
{
    public class Asn1ContextSpecific : Asn1StructuredObject
    {
        internal Asn1ContextSpecific(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("ContextSpecific: Tag = 0x{0:X2}", base.TaggedObject.Tag.Value);
        }
    }
}
