using System;

namespace Delta.CapiNet.Asn1
{
    internal class Asn1CharacterString : Asn1Utf8String
    {
        public Asn1CharacterString(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("CharacterString: {0}", base.Value);
        }
    }
}
