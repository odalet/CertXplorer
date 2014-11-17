using System;
using Delta.CapiNet.Logging;

namespace Delta.CapiNet.Asn1
{
    public class Asn1ObjectFactory
    {
        private static ILogService log = LogManager.GetLogger(typeof(Asn1ObjectFactory));

        public Asn1Object CreateAsn1Object(
            Asn1Document document, 
            TaggedObject content, 
            Asn1Object parent)
        {
            Asn1Object result = null;
            try
            {
                if (document.ShowInvalidTaggedObjects && IsInvalidTaggedObject(content))
                    return new Asn1InvalidObject(document, (InvalidTaggedObject)content, parent);

                var tagValue = content.Tag.Value;
                if (!tagValue.IsUniversalClass())
                {
                    var specific = CreateSpecificTaggedObject(document, content, parent);
                    return specific ?? CreateUnsupportedTaggedObject(document, content, parent);
                }

                var asn1TagValue = tagValue.GetAsn1TagValue();
                switch (asn1TagValue)
                {
                    case (int)Asn1Tags.Boolean:
                        return new Asn1Boolean(document, content, parent);
                    case (int)Asn1Tags.Integer:
                        return new Asn1Integer(document, content, parent);
                    case (int)Asn1Tags.BitString:
                        return new Asn1BitString(document, content, parent);
                    case (int)Asn1Tags.OctetString:
                        return new Asn1OctetString(document, content, parent);
                    case (int)Asn1Tags.Null:
                        return new Asn1Null(document, content, parent);
                    case (int)Asn1Tags.ObjectIdentifier:
                        return new Asn1Oid(document, content, parent);
                    case (int)Asn1Tags.Utf8String:
                        return new Asn1Utf8String(document, content, parent);
                    case (int)Asn1Tags.Sequence:
                        return new Asn1Sequence(document, content, parent);
                    case (int)Asn1Tags.Set:
                        return new Asn1Set(document, content, parent);
                    case (int)Asn1Tags.NumericString:
                        return new Asn1NumericString(document, content, parent);
                    case (int)Asn1Tags.PrintableString:
                        return new Asn1PrintableString(document, content, parent);
                    case (int)Asn1Tags.UtcTime:
                        return new Asn1UtcTime(document, content, parent);
                    default:
                        var specific = CreateSpecificTaggedObject(document, content, parent);                        
                        return specific ?? CreateUnsupportedTaggedObject(document, content, parent);
                }
            }
            catch (Exception ex)
            {
                // Could not create a "real" ASN1 object --> return the special "Unsupported" object.
                log.Error(ex);
                result = CreateUnsupportedTaggedObject(document, content, parent, ex);
            }

            return result;
        }

        protected virtual Asn1Object CreateSpecificTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent)
        {
            return null;
        }

        private Asn1Object CreateUnsupportedTaggedObject(Asn1Document document, TaggedObject content, Asn1Object parent, Exception exception = null)
        {
            if (exception != null)
                return new Asn1Unsupported(document, content, parent, exception);

            var tagValue = content.Tag.Value;
            if (tagValue.IsContextSpecificClass())
                return new Asn1ContextSpecific(document, content, parent);

            return new Asn1Unsupported(document, content, parent);
        }

        private static bool IsInvalidTaggedObject(TaggedObject content)
        {
            return content == null || content is InvalidTaggedObject;
        }
    }
}
