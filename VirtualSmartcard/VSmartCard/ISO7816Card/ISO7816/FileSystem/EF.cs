using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISO7816.FileSystem
{
    [Serializable]
    public abstract class EF : CardSelectable, IObjectWithFCI
    {
        public abstract uint Size { get; set; }

        public EF(ushort ID, IISO7816Card owner, DF parent)
        {
            id = ID;
            this.owner = owner;
            this.parent = parent;
            parent.Childs.Add(this);
            owner.ObjectChanged(this, ChangeType.Created);
            AC = owner.CreateAC(this);
            SM = owner.CreateSM(this);
        }
    }
    [Serializable]
    public class EFBinary : EF, IObjectWithData
    {
        public override string Description
        {
            get
            {
                return "Binary";
            }
        }
        public override uint Size { get { return (uint)data.Length; }
            set {
                byte[] newData = new byte[value];
                Array.Copy(data,0,newData,0,Math.Min(data.Length,newData.Length));
                data=newData;
            }
        }

        public override List<ISO7816.TLV.TLVelem> FCI
        {
            get
            {
                TLV tlv = new TLV();
                tlv.addTag(0x80, Util.ToByteArray((ushort)Data.Length));
                tlv.addTag(0x82, new byte[] { 0x01, 0, 0 });
                tlv.addTag(0x83, Util.ToByteArray(id));
                tlv.addTag(0x85, Util.ToByteArray((byte)1));
                tlv.addTag(0x89, Card.FullPath(this));
                return tlv.elems;
            }
        }

        internal byte[] data;
        public byte[] Data { get { return data; } set { data = value; } }

        public EFBinary(ushort ID, IISO7816Card owner, DF parent,uint size)
            : base(ID, owner, parent)
        {
            data = new byte[size];
        }
    }

    [Serializable]
    public abstract class EFRecord : EF, IObjectWithDataRecords
    {
        public override List<ISO7816.TLV.TLVelem> FCI
        {
            get
            {
                TLV tlv = new TLV();
                tlv.addTag(0x80, Util.ToByteArray((ushort)100));
                tlv.addTag(0x83, Util.ToByteArray(id));
                tlv.addTag(0x85, Util.ToByteArray((byte)1));
                tlv.addTag(0x89, Card.FullPath(this));
                return tlv.elems;
            }
        }

        public override uint Size { get { return size; } set {
            int curSize = 0;
            foreach (var v in data)
                curSize += v.Length;
            if (curSize > value)
                throw new Exception("Currently allocated size is" + curSize.ToString("X") + ". Smaller size is not allowed");
            size = value; 
        } }
        protected uint size;

        public abstract void Update(int recNum, byte[] data);
        public abstract byte[] Read(int recNum);
        public abstract int Append(byte[] data);

        protected IList<byte[]> data;
        public IList<byte[]> Data
        {
            get { return data; }
            set
            {
                if (data != value)
                {
                    data = value;
                    owner.ObjectChanged(this, ChangeType.Modified);
                }
            }
        }

        public EFRecord(ushort ID, IISO7816Card owner, DF parent)
            : base(ID, owner, parent)
        {
            data = new List<byte[]>();
        }
    }

    [Serializable]
    public class EFCyclic : EFRecord, IObjectWithRecordSize
    {
        public override string Description
        {
            get
            {
                return "Cyclic Fixed";
            }
        }

        public override List<TLV.TLVelem> FCI
        {
            get
            {
                var fci=base.FCI;
                fci.Add(new TLV.TLVelem(0x82,new byte[] {0x06,0,0}));
                return fci;
            }
        }
        int OldestRecord { get; set; }
        public int RecordSize
        {
            get { return recordSize; }
            set
            {
                int newSize = data.Count * value;
                if (newSize > Size)
                    throw new Exception("New allocated size would be " + newSize.ToString("X") + ", that is bigger than file Size.");
                for (int i = 0; i < data.Count; i++) {
                    byte[] newData = new byte[value];
                    Array.Copy(data[i], 0, newData, 0, Math.Min(data[i].Length, newData.Length));
                    data[i] = newData;
                    recordSize = value;
                }
            }
        }
        int recordSize;

        int NormRec(int recNum) {
            while (recNum < 0) recNum += (int)maxRecords;
            while (recNum >= maxRecords) recNum -= (int)maxRecords;
            return recNum;
        }
        public override void Update(int recNum, byte[] data)
        {
            if (data.Length != recordSize)
                throw new ISO7816Exception(Error.WrongLength);

            if (recNum < 0 || recNum >= this.data.Count)
                throw new ISO7816Exception(Error.RecordNotFound);

            if (this.data.Count == maxRecords)
                recNum = NormRec(OldestRecord - recNum);
            else
                recNum = OldestRecord - recNum;

            if (recNum < 0)
                throw new ISO7816Exception(Error.RecordNotFound);

            OldestRecord = recNum;
            this.data[recNum] = data;
        }
        public override byte[] Read(int recNum) {
            if (recNum < 0 || recNum >= this.data.Count)
                throw new ISO7816Exception(Error.RecordNotFound);

            if (this.data.Count == maxRecords)
                recNum = NormRec(OldestRecord - recNum);
            else
                recNum = OldestRecord - recNum;

            if (recNum < 0)
                throw new ISO7816Exception(Error.RecordNotFound);

            return this.data[recNum];
        }
        public override int Append(byte[] data) {
            if (data.Length != recordSize)
                throw new ISO7816Exception(Error.WrongLength);

            if (this.data.Count >= maxRecords)
            {
                OldestRecord = NormRec(OldestRecord + 1);
                this.data[OldestRecord] = data;
                owner.ObjectChanged(this, ChangeType.Modified);
                return 0;
            }
            else
            {
                this.data.Add(data);
                OldestRecord = this.data.Count-1;
                owner.ObjectChanged(this, ChangeType.Modified);
                return 0;
            }
        }
        uint maxRecords;
        public EFCyclic(ushort ID, IISO7816Card owner, DF parent, uint size, byte recordSize)
            : base(ID, owner, parent)
        {
            this.recordSize = recordSize;
            OldestRecord = 0;
            maxRecords = size / recordSize;
            this.size = size;
        }
    }

    [Serializable]
    public class EFLinearFixed : EFRecord, IObjectWithRecordSize
    {
        public override string Description
        {
            get
            {
                return "Linear Fixed";
            }
        }
        
        public override List<TLV.TLVelem> FCI
        {
            get
            {
                var fci=base.FCI;
                fci.Add(new TLV.TLVelem(0x82,new byte[] {0x02,0,0}));
                return fci;
            }
        }

        public int RecordSize
        {
            get { return recordSize; }
            set
            {
                int newSize = data.Count * value;
                if (newSize > Size)
                    throw new Exception("New allocated size would be " + newSize.ToString("X") + ", that is bigger than file Size.");
                for (int i = 0; i < data.Count; i++)
                {
                    byte[] newData = new byte[value];
                    Array.Copy(data[i], 0, newData, 0, Math.Min(data[i].Length, newData.Length));
                    data[i] = newData;
                }
                recordSize = value;
            }
        }
        int recordSize;

        public override void Update(int recNum, byte[] data) {
            if (recNum < 0 || recNum >= this.data.Count)
                throw new ISO7816Exception(Error.RecordNotFound);

            if (data.Length!=recordSize)
                throw new ISO7816Exception(Error.WrongLength);

            this.data[recNum] = data;
        }
        public override byte[] Read(int recNum)
        {
            if (recNum < 0 || recNum >= this.data.Count)
                throw new ISO7816Exception(Error.RecordNotFound);

            return this.data[recNum];
        }
        public override int Append(byte[] data) {
            if (data.Length != recordSize)
                throw new ISO7816Exception(Error.WrongLength);

            if (this.data.Count == 254)
                throw new ISO7816Exception(Error.InternalError);

            if (this.data.Count >= maxRecords)
                throw new ISO7816Exception(Error.NotEnoughSpace);

            this.data.Add(data);
            owner.ObjectChanged(this, ChangeType.Modified);
            return this.data.Count - 1;
        }
        uint maxRecords;
        public EFLinearFixed(ushort ID, IISO7816Card owner, DF parent, uint size, byte recordSize)
            : base(ID, owner, parent) {
                this.recordSize = recordSize;
                maxRecords = size / recordSize;
                this.size = size;
            }
    }
    [Serializable]
    public class EFLinearTLV : EFRecord {
        public override string Description
        {
            get
            {
                return "Linear TLV";
            }
        }

        public override List<TLV.TLVelem> FCI
        {
            get
            {
                var fci=base.FCI;
                fci.Add(new TLV.TLVelem(0x82,new byte[] {0x05,0,0}));
                return fci;
            }
        }
        int totSize = 0;
        public override void Update(int recNum, byte[] data) {
            if (recNum < 0 || recNum >= this.data.Count)
                throw new ISO7816Exception(Error.RecordNotFound);

            try
            {
                TLV tlv = new TLV(data);
            }
            catch
            {
                throw new ISO7816Exception(Error.RecordInconsistentWithTLVDataStructure);
            }

            if (totSize - this.data[recNum].Length + data.Length > size)
                throw new ISO7816Exception(Error.NotEnoughSpace);

            this.data[recNum] = data;
        }
        public override byte[] Read(int recNum) {
            if (recNum < 0 || recNum >= this.data.Count)
                throw new ISO7816Exception(Error.RecordNotFound);

            return this.data[recNum];
        }
        public override int Append(byte[] data) {
            try {
                TLV tlv = new TLV(data);
            }
            catch {
                throw new ISO7816Exception(Error.RecordInconsistentWithTLVDataStructure);
            }

            if (totSize + data.Length > size)
                throw new ISO7816Exception(Error.NotEnoughSpace);

            this.data.Add(data);
            totSize += data.Length;
            owner.ObjectChanged(this, ChangeType.Modified);
            return this.data.Count - 1;
        }
        public EFLinearTLV(ushort ID, IISO7816Card owner, DF parent, uint size)
            : base(ID, owner, parent) {
                this.size = size;
        }

        public int? SearchRecord(byte tag)
        {
            return SearchRecord(tag, 0);
        }
        public int? SearchRecord(int start, int tag)
        {
            for(int i=start;i<Data.Count;i++)
                if (data[i][0]==tag)
                    return i;
            return null;
        }
        public int? SearchRecordRev(byte tag)
        {
            return SearchRecordRev(tag, Data.Count-1);
        }
        public int? SearchRecordRev(int start, int tag)
        {
            for (int i = start; i >= 0; i++)
                if (data[i][0] == tag)
                    return i;
            return null;
        }
    }
}
