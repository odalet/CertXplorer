using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISO7816.FileSystem
{
    public enum SecurityEnvironmentComponent : byte
    {
        CON = 0xb8,
        TEST = 0xa4,
        CDS = 0xb6
    }

    [Serializable]
    public class SecurityEnvironmenet : ICardObject, IObjectWithData, IObjectWithAC
    {
        public string Description
        {
            get
            {
                return "Security Environment";
            }
        }

        public string FullPath { get { return ByteArray.hexDump(Card.FullPath(this)); } }

        public AC ac;
        public AC AC
        {
            get { return ac; }
            set
            {
                ac = value;
                owner.ObjectChanged(this, ChangeType.Modified);
            }
        }

        [NonSerialized]
        IISO7816Card owner;
        public IISO7816Card Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        DF parent;
        public DF Parent { get { return parent; } }

        internal byte[] data;
        public byte[] Data
        {
            get { return data; }
            set
            {
                data = value;
                owner.ObjectChanged(this, ChangeType.Modified);
            }
        }

        ushort id;
        public ushort ID
        {
            get { return (ushort)(0xff00 | id); }
            set { id = (ushort)(value & 0xff); }
        }

        public byte GetComponent(SecurityEnvironmentComponent comp) {
            switch (comp) { 
                case SecurityEnvironmentComponent.CDS:
                    if (Data != null && Data.Length >= 2)
                        return Data[1];
                    else
                        return 0x00;
                case SecurityEnvironmentComponent.CON:
                    if (Data != null && Data.Length >= 4)
                        return Data[3];
                    else
                        return 0x00;
                case SecurityEnvironmentComponent.TEST:
                    if (Data != null && Data.Length >= 5)
                        return Data[4];
                    else
                        return 0x00;
                default:
                    throw new ISO7816Exception(Error.InternalError);
            }
        }

        public SecurityEnvironmenet(byte ID, IISO7816Card owner, DF parent)
        {
            id = ID;
            this.owner = owner;
            this.parent = parent;
            parent.Childs.Add(this);
            owner.ObjectChanged(this, ChangeType.Created);
            AC = owner.CreateAC(this);
        }
    }
}
