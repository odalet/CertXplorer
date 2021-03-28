using System;

namespace Delta.CapiNet.Asn1
{
    public sealed class Asn1Unsupported : Asn1Object
    {
        public Asn1Unsupported(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : this(document, content, parentObject, null) { }

        internal Asn1Unsupported(Asn1Document document, TaggedObject content, Asn1Object parentObject, Exception exception)
            : base(document, content, parentObject) => Exception = exception;

        private Exception Exception { get; }

        public override string ToString()
        {
            var tagValue = base.TaggedObject.Tag.Value;

            var kind = tagValue.IsPrimitiveKind() ? "Primitive" : "Constructed";
            var cl = tagValue.GetAsn1ClassName();
            var t = string.Format("0x{0:X2}", tagValue);

            var baseMessage = $"Unsupported tag: {cl}/{kind}/{t}";
            return Exception == null ? 
                baseMessage :
                $"{baseMessage}\r\n\r\nException:\r\n{Exception}";
        }
    }
}
