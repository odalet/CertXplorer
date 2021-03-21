using System;
using System.Collections.Generic;

namespace Delta.CapiNet.Asn1
{
    public class Asn1Document
    {
        public Asn1Document(byte[] data, bool parseOctetStrings, bool showInvalidTaggedObjects) :
            this(data, parseOctetStrings, showInvalidTaggedObjects, new Asn1ObjectFactory()) { }

        protected Asn1Document(
            byte[] data, bool parseOctetStrings, bool showInvalidTaggedObjects, Asn1ObjectFactory objectFactory)
        {
            ObjectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));

            Data = data;
            ParseOctetStrings = parseOctetStrings;
            ShowInvalidTaggedObjects = showInvalidTaggedObjects;

            var taggedObjects = TaggedObject.CreateObjects(data, 0, data.Length);
            if (taggedObjects != null && taggedObjects.Length > 0)
            {
                var objects = new List<Asn1Object>();
                foreach (var taggedObject in taggedObjects)
                {
                    var asn1 = ObjectFactory.CreateAsn1Object(this, taggedObject, null);
                    if (asn1 != null) 
                        objects.Add(asn1);
                }

                Nodes = objects.ToArray();
            }
            else Nodes = new Asn1Object[0];
        }

        public byte[] Data { get; }
        public Asn1Object[] Nodes { get; }
        public bool ParseOctetStrings { get; }
        public bool ShowInvalidTaggedObjects { get; }        
        internal Asn1ObjectFactory ObjectFactory { get; }
    }
}
