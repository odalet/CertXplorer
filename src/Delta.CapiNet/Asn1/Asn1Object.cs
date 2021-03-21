using System;

namespace Delta.CapiNet.Asn1
{
    public abstract class Asn1Object
    {        
        protected Asn1Object(Asn1Document document, TaggedObject content, Asn1Object parentObject)
        {
            TaggedObject = content ?? throw new ArgumentNullException(nameof(content));
            Document = document;
            Parent = parentObject;
        }

        public Asn1Document Document { get; }
        public Asn1Object Parent { get; }
        public byte[] RawData => TaggedObject.RawData;
        public byte[] Workload => TaggedObject.Workload;
        public int WorkloadOffset => TaggedObject.WorkloadOffset;
        protected internal TaggedObject TaggedObject { get; }

        public virtual void Initialize() { /* Inheritors may fill this */ }
    }
}
