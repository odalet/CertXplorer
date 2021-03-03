using Delta.CapiNet.Asn1;

namespace Delta.Icao.Asn1
{
    public sealed class Asn1DG1Data : Asn1DataGroupData
    {
        private sealed class Factory : Asn1ObjectFactory
        {
            protected override Asn1Object CreateSpecificTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent)
            {
                var result = base.CreateSpecificTaggedObject(document, content, parent);
                if (result != null)
                    return result;

                var tag = content.Tag.Value;
                if (tag == (ushort)Tags.Icao.Mrz)
                    return new Asn1Mrz(document, content, parent);

                return null;
            }
        }

        private static readonly Asn1ObjectFactory factory = new Factory();
        protected override Asn1ObjectFactory ObjectFactory => factory;

        public Asn1DG1Data(Asn1Document document, TaggedObject content, Asn1Object parentObject) : base(document, content, parentObject) { }
    }
}
