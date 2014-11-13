using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISO7816.FileSystem
{
    public class Typed<T> {
        public Typed() { }
        T val;
        public static implicit operator Typed<T>(T b)
        {
            var t=new Typed<T>(b);
            return t;
        }
        public static implicit operator T(Typed<T> b)
        {
            return b.val;
        }
        protected Typed(T b)
        {
            this.val = b;
        }
    }
    public class BSOClass : Typed<byte>
    {
        internal static SortedList<byte, string> values = new SortedList<byte, string>();
        public const byte Unspecified = 0xff;
        public const byte Test = 0;
        public const byte TestComponent2 = 1;
        public const byte SM = 0x10;
        public const byte PSO = 0x20;
        public const byte PSOComponent2 = 0x21;
        static BSOClass()
        {
            values[Unspecified] = "Unspecified";
            values[Test] = "Test";
            values[TestComponent2] = "TestComponent2";
            values[SM] = "SM";
            values[PSO] = "PSO";
            values[PSOComponent2] = "PSOComponent2";
        }
        public BSOClass(byte b) : base(b) { }
    }
    public class BSOAlgo : Typed<byte>
    {
        internal static SortedList<byte, string> values = new SortedList<byte, string>();
        public const byte Unspecified = 0xff;
        public const byte RSA_Enc = 0x0c;
        public const byte RSA_DS_Test = 0x88;
        public const byte DES3_Enc_SMEnc = 0x03;
        public const byte MAC3_Test_SMSig = 0x82;
        public const byte PIN = 0x87;
        public const byte Logic = 0x7f;
        static BSOAlgo() {
            values[Unspecified] = "Unspecified";
            values[RSA_Enc] = "RSA_Enc";
            values[RSA_DS_Test] = "RSA_DS_Test";
            values[DES3_Enc_SMEnc] = "DES3_Enc_SMEnc";
            values[MAC3_Test_SMSig] = "MAC3_Test_SMSig";
            values[PIN] = "PIN";
            values[Logic] = "Logic";
        }
        public BSOAlgo(byte b) : base(b) { }
    }
    [Serializable]
    public class BSO : ICardObject, IObjectWithData, IObjectWithAC, IObjectWithSM
    {
        public string Description { get {
            switch (Class) { 
                case BSOClass.Test:
                case BSOClass.TestComponent2:
                    return "Test";
                case BSOClass.SM:
                    return "Secure Messaging";
                case BSOClass.PSO:
                case BSOClass.PSOComponent2:
                    return "PSO";
                default:
                    return "Unknown type";
            }
        } }

        public string FullPath { get { return ByteArray.hexDump(Card.FullPath(this)); } }

        public BSOClass Class { get { 
            return new BSOClass(Util.UpperByte(ID));
        }
            set {
                id = Util.ToUShort(value, Util.LowerByte(ID));
            }
        }
        public byte KeyID { get { 
            return Util.LowerByte(ID); 
        }
            set {
                if (value <1 || value == 0x1F)
                    throw new Exception("BSO ID " +value.ToString("X02") + " is reserved");

                id = Util.ToUShort(Util.UpperByte(id), value);
            }
        }        
        public BSOAlgo Algo { get { 
            return new BSOAlgo(options[2]);
        }
            set { options[2] = value; }
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

        byte[] options;
        public byte[] Options
        {
            get {
                return options;
            }
            set
            {
                if (value.Length != 8)
                    throw new Exception("Invalid Options Length");
                options = value;
                CurErrorCounter = (MaxErrorCounter != 0 && MaxErrorCounter != 0x0f) ? MaxErrorCounter : 0x0f;
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
            set {
                if (Algo==BSOAlgo.PIN && value.Length<MinLength)
                    throw new ISO7816Exception(Error.WrongLength);

                if (Algo == BSOAlgo.RSA_DS_Test ||
                    Algo == BSOAlgo.RSA_Enc) {
                        if (value[0]!=0xff && value[0] != value.Length - 1)
                            throw new ISO7816Exception(Error.DataFieldNotValid);
                }
                data = value;
                owner.ObjectChanged(this, ChangeType.Modified);
            }
        }

        public byte MinLength
        {
            get
            {
                if (options == null)
                    throw new ISO7816Exception(Error.InternalError);
                return options[7];
            }
            set {
                if (Algo == BSOAlgo.PIN)
                {
                    if (value > Data.Length)
                        throw new Exception("Current PIN size is " + Data.Length.ToString("X") + ". Bigger MinLength is not allowed");
                }
                else if (Class == BSOClass.Test && (Algo == BSOAlgo.MAC3_Test_SMSig ||
                    Algo == BSOAlgo.RSA_DS_Test))
                {
                    if ((value % 8) != 0)
                        throw new Exception("Challenge length must be multiple of 8 bytes");
                }
                else { 
                    if (value!=0)
                        throw new Exception("For objects other than PINS and C/R, MinLength must be 0");

                }
                options[7] = value; 
            }
        }
        

        protected ushort id;
        public ushort ID
        {
            get { return id; }
        }

        public bool Blocked { get; set; }

        public int CurValidityCounter {get;set;}
        public int ValidityCounter
        {
            get
            {
                if (options == null)
                    throw new ISO7816Exception(Error.InternalError);
                return options[6];
            }
            set {
                options[6] = (byte)value;
            }
        }

        public int CurErrorCounter { get; set; }
        public int MaxErrorCounter
        {
            get
            {
                if (options == null)
                    throw new ISO7816Exception(Error.InternalError);
                return options[3];
            }
            set {
                options[3] = (byte)value;
            }
        }

        public BSO(ushort ID, IISO7816Card owner, DF parent)
        {
            id = ID;
            this.owner = owner;
            this.parent = parent;
            Blocked = false;
            options = new byte[8];
            parent.Childs.Add(this);
            owner.ObjectChanged(this, ChangeType.Created);
            AC = owner.CreateAC(this);
            SM = owner.CreateSM(this);
        }
    }
}
