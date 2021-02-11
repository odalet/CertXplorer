using System;

namespace Delta.CapiNet.Asn1
{
    public class Asn1Oid : Asn1Object
    {
        private string oid = string.Empty;

        public Asn1Oid(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject)
        {
            oid = OidUtils.Decode(content.Workload);
        }

        /// <summary>
        /// Gets this instance's value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get { return oid; } }

        public string Name 
        {
            get 
            {
                var name = OidUtils.GetOidName(oid);
                if (string.IsNullOrEmpty(name)) name = "Unknown";
                return name;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <remarks>
        /// This forces inheritors to explicitly implement ToString.
        /// </remarks>
        public override string ToString()
        {
            return string.Format("Oid: {0} ({1})", Name, Value);
        }
    }
}
