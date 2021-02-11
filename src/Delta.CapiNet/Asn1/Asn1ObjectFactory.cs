using System;
using System.Linq;
using Delta.CapiNet.Logging;

namespace Delta.CapiNet.Asn1
{
    public class Asn1ObjectFactory
    {
        private static ILogService log = LogManager.GetLogger(typeof(Asn1ObjectFactory));

        public Asn1Object CreateAsn1Object(Asn1Document document, TaggedObject content, Asn1Object parent)
        {
            try
            {
                if (IsInvalidTaggedObject(content))
                {
                    if (document.ShowInvalidTaggedObjects)
                        return new Asn1InvalidObject(document, (InvalidTaggedObject)content, parent);
                    return null;
                }

                var tagValue = content.Tag.Value;
                if (!tagValue.IsUniversalClass())
                {
                    var specific = CreateSpecificTaggedObject(document, content, parent);
                    return specific ?? CreateUnsupportedTaggedObject(document, content, parent);
                }

                var asn1TagValue = tagValue.GetAsn1TagValue();
                if (IsAsn1Tag(asn1TagValue))
                    return CreateUniversalTaggedObject((Asn1Tag)asn1TagValue, document, content, parent);

                // Otherwise
                return CreateSpecificOrUnsupportedTaggedObject(document, content, parent);
            }
            catch (Exception ex)
            {
                // Could not create a "real" ASN1 object --> return the special "Unsupported" object.
                log.Error(ex);
                return CreateUnsupportedTaggedObject(document, content, parent, ex);
            }
        }

        protected Asn1Object CreateUniversalTaggedObject(Asn1Tag asn1Tag, Asn1Document document, TaggedObject content, Asn1Object parent)
        {
            switch (asn1Tag)
            {
                case Asn1Tag.Boolean:
                    return new Asn1Boolean(document, content, parent);
                case Asn1Tag.Integer:
                    return new Asn1Integer(document, content, parent);
                case Asn1Tag.BitString:
                    return new Asn1BitString(document, content, parent);
                case Asn1Tag.CharacterString:
                    return new Asn1CharacterString(document, content, parent);
                case Asn1Tag.OctetString:
                    return new Asn1OctetString(document, content, parent);
                case Asn1Tag.Null:
                    return new Asn1Null(document, content, parent);
                case Asn1Tag.ObjectIdentifier:
                    return new Asn1Oid(document, content, parent);
                case Asn1Tag.Utf8String:
                    return new Asn1Utf8String(document, content, parent);
                case Asn1Tag.Sequence:
                    return new Asn1Sequence(document, content, parent);
                case Asn1Tag.Set:
                    return new Asn1Set(document, content, parent);
                case Asn1Tag.NumericString:
                    return new Asn1NumericString(document, content, parent);
                case Asn1Tag.PrintableString:
                    return new Asn1PrintableString(document, content, parent);
                case Asn1Tag.UtcTime:
                    return new Asn1UtcTime(document, content, parent);                
            }

            return null;
        }

        protected virtual Asn1Object CreateSpecificTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent)
        {
            return null;
        }

        protected bool IsAsn1Tag(int tagValue)
        {
            return Enum.GetValues(typeof(Asn1Tag)).Cast<Asn1Tag>().Select(t => (int)t).Any(v => v == tagValue);
        }

        protected Asn1Object CreateUnsupportedTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent, Exception exception = null)
        {
            if (exception != null)
                return new Asn1Unsupported(document, content, parent, exception);

            var tagValue = content.Tag.Value;
            if (tagValue.IsContextSpecificClass())
                return new Asn1ContextSpecific(document, content, parent);

            return new Asn1Unsupported(document, content, parent);
        }

        private Asn1Object CreateSpecificOrUnsupportedTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent)
        {
            var specific = CreateSpecificTaggedObject(document, content, parent);
            return specific ?? CreateUnsupportedTaggedObject(document, content, parent);
        }

        private static bool IsInvalidTaggedObject(TaggedObject content)
        {
            return content == null || content is InvalidTaggedObject;
        }
    }
}
