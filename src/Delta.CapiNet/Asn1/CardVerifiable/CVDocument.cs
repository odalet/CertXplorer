using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Delta.CapiNet.Asn1.CardVerifiable
{
    public class CVDocument : Asn1Document
    {
        internal class CVTag
        {
            public CVTag(ushort id, Asn1Tag? asn1Type, string name)
            {
                Id = id;
                Asn1Type = asn1Type;
                Name = name;
            }

            public ushort Id { get; private set; }
            public Asn1Tag? Asn1Type { get; private set; }
            public string Name { get; private set; }

        }

        public class Factory : Asn1ObjectFactory
        {
            private static readonly Dictionary<ushort, CVTag> cvtags;
            static Factory()
            {
                // Fill the tags list. See Table 27 (Annex D.2) in BSI_TR-03110 Part 3 V2.2:
                // https://www.bsi.bund.de/SharedDocs/Downloads/EN/BSI/Publications/TechGuidelines/TR03110/BSI_TR-03110_Part-3-V2_2.pdf

                var tags = new List<CVTag>()
                {
                    new CVTag(0x42, Asn1Tag.CharacterString, "Certification Authority Reference"),
                    new CVTag(0x53, Asn1Tag.OctetString, "Discretionary Data"),
                    new CVTag(0x5F20, Asn1Tag.CharacterString, "Certificate Holder Reference"),
                    new CVTag(0x5F24, Asn1Tag.OctetString, "Certificate Expiration Date"), // TODO BCD YYMMDD
                    new CVTag(0x5F25, Asn1Tag.OctetString, "Certificate Effective Date"),
                    new CVTag(0x5F29, Asn1Tag.Integer, "Certificate Profile Identifier"), // TODO: Unsigned integer
                    new CVTag(0x5F37, Asn1Tag.OctetString, "Signature"),
                    new CVTag(0x65, Asn1Tag.Sequence, "Certificate Extensions"),
                    new CVTag(0x67, Asn1Tag.Sequence, "Authentication"),
                    new CVTag(0x73, Asn1Tag.Sequence, "Discretionary Data Template"),
                    new CVTag(0x7F21, Asn1Tag.Sequence, "CV Certificate"),
                    new CVTag(0x7F49, Asn1Tag.Sequence, "Public Key"),
                    new CVTag(0x7F4C, Asn1Tag.Sequence, "Certificate Holder Authorization Template"),
                    new CVTag(0x7F4E, Asn1Tag.Sequence, "Certificate Body")
                };

                cvtags = new Dictionary<ushort, CVTag>();
                foreach (var cvtag in tags)
                    cvtags.Add(cvtag.Id, cvtag);
            }

            protected override Asn1Object CreateSpecificTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent)
            {
                var asn1 = base.CreateSpecificTaggedObject(document, content, parent);
                if (asn1 != null)
                    return asn1;

                // CV Specific tags
                var tagValue = content.Tag.Value;
                if (cvtags.ContainsKey(tagValue))
                {
                    var cvtag = cvtags[tagValue];
                    if (cvtag.Asn1Type.HasValue)
                    {
                        return base.CreateUniversalTaggedObject(cvtag.Asn1Type.Value, document, content, parent);
                    }
                    else
                    {
                        // TODO!!!
                        return base.CreateUnsupportedTaggedObject(document, content, parent);
                    }
                }
                else return base.CreateUnsupportedTaggedObject(document, content, parent);
            }
        }

        private static readonly Asn1ObjectFactory factory = new Factory();

        public CVDocument(byte[] data, bool parseOctetStrings, bool showInvalidTaggedObjects) :
            base(data, parseOctetStrings, showInvalidTaggedObjects, factory)
        { }

    }
}
