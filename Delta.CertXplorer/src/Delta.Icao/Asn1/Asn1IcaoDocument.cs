using Delta.CapiNet.Asn1;
using Delta.Icao.Lds;

namespace Delta.Icao.Asn1
{
    public class Asn1IcaoDocument : Asn1Document
    {
        public class Factory : Asn1ObjectFactory
        {
            protected override Asn1Object CreateSpecificTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent)
            {
                var asn1 = base.CreateSpecificTaggedObject(document, content, parent);
                if (asn1 != null)
                    return asn1;

                var tagValue = content.Tag.Value;
                var ef = Tags.FindEF(tagValue);
                if (!ef.HasValue)
                    return null;

                switch (ef.Value)
                {
                    case Tags.EF.DG1:
                        return new Asn1DG1Data(document, content, parent);
                }

                return null;
            }
        }

        private static readonly Asn1ObjectFactory factory = new Factory();

        public Asn1IcaoDocument(byte[] data, bool parseOctetStrings, bool showInvalidTaggedObjects) :
            base(data, parseOctetStrings, showInvalidTaggedObjects, factory) { }
    }
}
