using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISO7816.FileSystem
{
    [Serializable]
    public class DF : CardSelectable, IObjectWithFCI
    {
        public override string Description
        {
            get
            {
                return "DF";
            }
        }

        public override List<ISO7816.TLV.TLVelem> FCI
        {
            get
            {
                TLV tlv = new TLV();
                tlv.addTag(0x81, Util.ToByteArray((ushort)0));
                tlv.addTag(0x82, new byte[] {0x38,0,0} );
                tlv.addTag(0x83, Util.ToByteArray(id));
                tlv.addTag(0x85, Util.ToByteArray((byte)1));
                tlv.addTag(0x89, Card.FullPath(this));
                return tlv.elems;
            }
        }

        internal byte[] aid = null;
        public byte[] AID
        {
            get { return aid; }
            set { aid = value; }
        }

        protected List<ICardObject> childs;
        public List<ICardObject> Childs { get { return childs; } }

        protected DF() {}
        public DF(ushort ID, IISO7816Card owner, DF parent)
        {
            id = ID;
            this.owner = owner;
            this.parent = parent;
            parent.Childs.Add(this);
            childs = new List<ICardObject>();
            owner.ObjectChanged(this, ChangeType.Created);
            AC = owner.CreateAC(this);
            SM = owner.CreateSM(this);
        }

        public ICardObject GetChildEF(ushort id)
        {
            if (!active)
                throw new ISO7816Exception(Error.FileNotFound);
            foreach (var v in childs)
            {
                if (v.ID == id)
                {
                    if (v is EF)
                        return v;
                }
            }
            return null;
        }
        public SecurityEnvironmenet GetChildSE(byte id)
        {
            return GetChildSE(id, false);
        }
        public SecurityEnvironmenet GetChildSE(byte id, bool backtrack)
        {
            if (!active)
                throw new ISO7816Exception(Error.FileNotFound);
            ushort Id = (ushort)(id | 0xff00);
            foreach (var v in childs)
            {
                if (v.ID == Id)
                {
                    if (v is SecurityEnvironmenet)
                        return v as SecurityEnvironmenet;
                }
            }
            if (!backtrack)
                return null;
            if (parent != null)
                return parent.GetChildSE(id, backtrack);
            return null;
        }

        public BSO GetChildBSO(byte id)
        {
            foreach (var v in childs)
            {
                if (v is BSO) {
                    var bso=v as BSO;
                    if (bso.KeyID == id)
                        return bso;
                }
            }
            return null;
        }

        public ICardObject GetChild(ushort id)
        {
            foreach (var v in childs)
            {
                if (v.ID == id)
                    return v;
            }
            return null;
        }


        public BSO GetChildBSO(ushort id) {
            return GetChildBSO(id, false);
        }
        public BSO GetChildBSO(ushort id,bool backtrack)
        {
            if (!active)
                throw new ISO7816Exception(Error.FileNotFound);
            foreach (var v in childs)
            {
                if (v.ID == id)
                {
                    if (v is BSO)
                        return v as BSO;
                }
            }
            if (!backtrack)
                return null;
            if (parent != null)
                return parent.GetChildBSO(id, backtrack);
            return null;
        }
        
        public DF GetChildDF(ushort id)
        {
            if (!active)
                throw new ISO7816Exception(Error.FileNotFound);
            foreach (var v in childs)
            {
                if (v.ID == id)
                {
                    if (v is DF)
                        return v as DF;
                }
            }
            return null;
        }

        public CardSelectable GetChildEForDF(ushort id)
        {
            if (!active)
                throw new ISO7816Exception(Error.FileNotFound);
            foreach (var v in childs)
            { 
                if (v.ID==id) {
                    if ((v is DF) || (v is EF))
                        return v as CardSelectable;
                }
            }
            return null;
        }
        public void RemoveChild(ICardObject obj) {
            int i = childs.IndexOf(obj);
            if (i >= 0)
            {
                childs.RemoveAt(i);
                owner.ObjectChanged(obj, ChangeType.Deleted);
            }
        }
    }

    [Serializable]
    public class MF : DF
    {
        public byte[] ATR
        {
            get { return Owner.ATR; }
        }

        public override string Description
        {
            get
            {
                return "Master File";
            }
        }

        public override ushort ID
        {
            get
            {
                return 0x3f00;
            }
            set
            {
                if (value!=0x3f00)
                    throw new Exception("MF should have ID 3F00");
            }
        }
        public MF(Card owner) {
            this.owner = owner;
            childs = new List<ICardObject>();
            owner.ObjectChanged(this,ChangeType.Created);
            AC = owner.CreateAC(this);
            SM = owner.CreateSM(this);
        }
    }
}
