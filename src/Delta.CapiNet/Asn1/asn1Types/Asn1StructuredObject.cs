﻿using System.Collections.Generic;

namespace Delta.CapiNet.Asn1
{
    public abstract class Asn1StructuredObject : Asn1Object
    {
        protected Asn1StructuredObject(Asn1Document document, TaggedObject content, Asn1Object parentObject)
            : base(document, content, parentObject) { }

        public override void Initialize() => ParseContent();

        public Asn1Object[] Nodes { get; protected set; }

        protected virtual Asn1ObjectFactory ObjectFactory => Document.ObjectFactory;

        protected virtual void ParseContent()
        {
            var taggedObjects = TaggedObject.CreateObjects(
                TaggedObject.AllData,
                TaggedObject.WorkloadOffset,
                TaggedObject.WorkloadLength);

            if (taggedObjects != null && taggedObjects.Length > 0)
            {
                var objects = new List<Asn1Object>();
                foreach (var taggedObject in taggedObjects)
                {
                    var asn1 = ObjectFactory.CreateAsn1Object(base.Document, taggedObject, this);
                    if (asn1 != null) objects.Add(asn1);
                }

                Nodes = objects.ToArray();
            }
            else Nodes = new Asn1Object[0];
        }
    }
}
