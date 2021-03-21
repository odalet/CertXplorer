using System;
using System.Linq;
using Delta.CapiNet.Logging;

namespace Delta.CapiNet.Asn1
{
    public class Asn1ObjectFactory
    {
        private static readonly ILogService log = LogManager.GetLogger(typeof(Asn1ObjectFactory));

        public Asn1Object CreateAsn1Object(Asn1Document document, TaggedObject content, Asn1Object parent)
        {
            var asn1Object = CreateUninitializedAsn1Object(document, content, parent);
            asn1Object.Initialize();
            return asn1Object;
        }

        protected Asn1Object CreateUniversalTaggedObject(
            Asn1Tag asn1Tag, Asn1Document document, TaggedObject content, Asn1Object parent) => asn1Tag switch
            {
                Asn1Tag.Boolean => new Asn1Boolean(document, content, parent),
                Asn1Tag.Integer => new Asn1Integer(document, content, parent),
                Asn1Tag.BitString => new Asn1BitString(document, content, parent),
                Asn1Tag.CharacterString => new Asn1CharacterString(document, content, parent),
                Asn1Tag.OctetString => new Asn1OctetString(document, content, parent),
                Asn1Tag.Null => new Asn1Null(document, content, parent),
                Asn1Tag.ObjectIdentifier => new Asn1Oid(document, content, parent),
                Asn1Tag.Utf8String => new Asn1Utf8String(document, content, parent),
                Asn1Tag.Sequence => new Asn1Sequence(document, content, parent),
                Asn1Tag.Set => new Asn1Set(document, content, parent),
                Asn1Tag.NumericString => new Asn1NumericString(document, content, parent),
                Asn1Tag.PrintableString => new Asn1PrintableString(document, content, parent),
                Asn1Tag.UtcTime => new Asn1UtcTime(document, content, parent),
                _ => null,
            };

        protected virtual Asn1Object CreateSpecificTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent) => null;

        protected Asn1Object CreateUnsupportedTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent, Exception exception = null)
        {
            if (exception != null)
                return new Asn1Unsupported(document, content, parent, exception);

            var tagValue = content.Tag.Value;
            return tagValue.IsContextSpecificClass()
                ? new Asn1ContextSpecific(document, content, parent)
                : new Asn1Unsupported(document, content, parent);
        }

        private Asn1Object CreateUninitializedAsn1Object(Asn1Document document, TaggedObject content, Asn1Object parent)
        {
            try
            {
                if (IsInvalidTaggedObject(content)) return document.ShowInvalidTaggedObjects ?
                        new Asn1InvalidObject(document, (InvalidTaggedObject)content, parent) :
                        null;

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

        private Asn1Object CreateSpecificOrUnsupportedTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent)
        {
            var specific = CreateSpecificTaggedObject(document, content, parent);
            return specific ?? CreateUnsupportedTaggedObject(document, content, parent);
        }

        private static bool IsAsn1Tag(int tagValue) => Enum
            .GetValues(typeof(Asn1Tag)).Cast<Asn1Tag>().Select(x => (int)x).Any(v => v == tagValue);

        private static bool IsInvalidTaggedObject(TaggedObject content) => content == null || content is InvalidTaggedObject;
    }
}
