using System;
using System.Collections.Generic;

namespace Delta.CapiNet.Asn1
{
    public class Asn1Document
    {
        private const byte tagMask = 0x1F;
        private const byte classMask = 0xC0;

        public Asn1Document(byte[] data, bool parseOctetStrings, bool showInvalidTaggedObjects) :
            this(data, parseOctetStrings, showInvalidTaggedObjects, null) { }

        protected Asn1Document(
            byte[] data, bool parseOctetStrings, bool showInvalidTaggedObjects, Asn1ObjectFactory objectFactory)
        {
            Data = data;
            ParseOctetStrings = parseOctetStrings;
            ShowInvalidTaggedObjects = showInvalidTaggedObjects;

            ObjectFactory = objectFactory ?? new Asn1ObjectFactory();

            var taggedObjects = CreateTaggedObjects(data);
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

        internal Asn1ObjectFactory ObjectFactory
        {
            get; private set;
        }

        public byte[] Data
        {
            get;
            private set;
        }

        public bool ParseOctetStrings
        {
            get;
            private set;
        }

        public bool ShowInvalidTaggedObjects
        {
            get;
            private set;
        }

        public Asn1Object[] Nodes
        {
            get;
            protected set;
        }

        public Asn1Object CreateAsn1Object(Asn1Document document,  TaggedObject content,  Asn1Object parent)
        {
            return ObjectFactory.CreateAsn1Object(document, content, parent);
        }

        #region Helpers

        private static TaggedObject[] CreateTaggedObjects(byte[] data)
        {
            return TaggedObject.CreateObjects(data, 0, data.Length);
        }

        #endregion
    }
}
