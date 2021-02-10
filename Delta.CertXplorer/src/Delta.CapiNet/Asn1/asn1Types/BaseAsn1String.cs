using System;
using System.Text;
using Delta.CapiNet.Logging;

namespace Delta.CapiNet.Asn1
{
    public abstract class BaseAsn1String : Asn1Object
    {
        private static readonly ILogService log = LogManager.GetLogger(typeof(BaseAsn1String));
        private string decoded = null;

        protected BaseAsn1String(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        public string Value
        {
            get
            {
                if (decoded == null) decoded = Decode(TaggedObject.Workload);
                return decoded;
            }
        }

        protected virtual string FriendlyName => "String";
        protected virtual Encoding Encoding => Encoding.UTF8;

        protected virtual string Decode(byte[] payload)
        {
            try
            {
                return Encoding.GetString(payload);
            }
            catch (Exception ex)
            {
                log.Error($"Could not decode input data using a {Encoding.EncodingName} encoding: {ex.Message}", ex);
                return $"DECODE ERROR";
            }            
        }

        public override string ToString() => $"{FriendlyName}: {Value}";
    }
}
