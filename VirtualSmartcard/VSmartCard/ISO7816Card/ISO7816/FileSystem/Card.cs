using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISO7816.FileSystem
{
    public interface IObjectWithFCI
    {
        List<TLV.TLVelem> FCI { get; }
    }

    public interface IObjectWithAC
    {
        AC AC { get; }
    }

    public interface IObjectWithSM
    {
        SM SM { get; }
    }

    public interface IObjectWithData
    {
        byte[] Data { get; set; }
    }

    public interface IObjectWithDataRecords
    {
        IList<byte[]> Data { get; }
    }

    public interface IObjectWithRecordSize
    {
        int RecordSize { get; set; }
    }

    public interface ICardObject
    {
        string Description { get; }
        string FullPath { get; }
        DF Parent { get; }
        IISO7816Card Owner { get; set; }
        ushort ID { get; }
    }
    [Serializable]
    public abstract class CardSelectable : ICardObject, IObjectWithAC, IObjectWithSM
    {
        public abstract string Description { get; }
        public string FullPath { get { return ByteArray.hexDump(Card.FullPath(this)); } }

        protected IISO7816Card owner;
        public IISO7816Card Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        protected DF parent;
        public DF Parent { get { return parent; } }

        public abstract List<ISO7816.TLV.TLVelem> FCI { get; }

        protected ushort id;
        public virtual ushort ID
        {
            get { return id; }
            set {
                if (value == 0x3F00 || value == 0x3FFF || value == 0xFFFF)
                    throw new Exception("The ID " +value.ToString("X04") + " is reserved");
                if (parent!=null && parent.GetChildEForDF(value)!=null)
                    throw new Exception("ID " + value.ToString("X04") + " is already used in parent DF");

                id = value;
            }
        }

        protected bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                owner.ObjectChanged(this, ChangeType.Modified);
            }
        }

        public AC ac;
        public SM sm;
        public AC AC
        {
            get { return ac; }
            set
            {
                ac = value;
                owner.ObjectChanged(this, ChangeType.Modified);
            }
        }
        public SM SM
        {
            get { return sm; }
            set
            {
                sm = value;
                owner.ObjectChanged(this, ChangeType.Modified);
            }
        }

        public CardSelectable() {
            active = true;
        }
    }
}
