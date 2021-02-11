using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;
using ISO7816.Handler;
using System.IO.Pipes;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using System.Xml.Serialization;
using VirtualSmartCard;
using CardModule;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace ISO7816
{
    public enum ChangeType
    {
        Selected,
        Unselected,
        Created,
        Modified,
        Deleted
    }
    public interface IObservable<S,T>
    {
        event Action<S, T, ChangeType> objectChanged;
    }

    public class IISO7816Implementation : ICardImplementation
    {

        public ICard GetCardObject()
        {
            return new Card();
        }

        public override string ToString()
        {
            return "ISO 7816 Card";
        }
    }
    public class CardPlugin : ICardPlugin {
        static public ICardImplementation ISO7816Implementation = new IISO7816Implementation();
        ICardImplementation[] implementations = new ICardImplementation[] {
            ISO7816Implementation
        };
        public IEnumerable<ICardImplementation> Implementations
        {
            get { return implementations; }
        }
    }

    public interface IISO7816Card :ICard {
        MF MasterFile { get; }

        BSO CreateBSO(ushort id, DF parent);
        DF CreateDF(ushort id, DF parent);
        EF CreateEFLinearTLV(ushort id, DF parent, uint size);
        EF CreateEF(ushort id, DF parent, uint size);
        SecurityEnvironmenet CreateSE(byte id, DF parent);
        event Action<ICard, ICardObject, ChangeType> objectChanged;
        void ObjectChanged(ICardObject obj, ChangeType type);

        bool CheckBSOId(byte Id);

        AC CreateAC(IObjectWithAC dest);
        SM CreateSM(IObjectWithSM dest);

        byte[] GetSMTLV(TLV tlv);
    }

    [Serializable]
    public class Card : IObservable<ICard, ICardObject>, IISO7816Card, IDeserializationCallback
    {
        public virtual ICardImplementation Implementation { get {return CardPlugin.ISO7816Implementation; }}

        public Control GetUI() {
            var UI=new FileSystemUI();
            UI.SetCard(this);
            return UI;
        }
        public virtual bool CheckBSOId(byte id) {
            return (id != 0 && id <= 0x1f);
        }
        public virtual byte[] GetSMTLV(TLV tlv) {
            return tlv[0xcb];
        }
        public virtual AC CreateAC(IObjectWithAC dest) {
            int numAC=0;
            if (dest is BSO)
                numAC = 7;
            if (dest is DF)
                numAC = 9;
            if (dest is EF)
                numAC = 9;
            if (dest is SecurityEnvironmenet)
                numAC = 2;
            if (numAC == 0)
                throw new Exception("Oggetto con AC non valido");
            return new AC(AC.Always, dest, numAC);
        }
        public virtual SM CreateSM(IObjectWithSM dest)
        {
            int numSM = 0;
            if (dest is BSO)
                numSM = 16;
            if (dest is DF)
                numSM = 24;
            if (dest is EF)
                numSM = 24;
            if (numSM == 0)
                throw new Exception("Oggetto con SM non valido");
            return new SM(SM.NoSM, SM.NoSM, dest, numSM);
        }
        public virtual EF CreateEF(ushort id, DF parent, uint size)
        {
            return new EFBinary(id, this, parent, size);
        }
        public virtual EF CreateEFLinearTLV(ushort id, DF parent, uint size)
        {
            return new EFLinearTLV(id, this, parent, size);
        }

        public virtual DF CreateDF(ushort id, DF parent)
        {
            return new DF(id, this, parent);
        }
        public virtual BSO CreateBSO(ushort id, DF parent)
        {
            return new BSO(id, this, parent);
        }
        public virtual SecurityEnvironmenet CreateSE(byte id, DF parent)
        {
            return new SecurityEnvironmenet(id, this, parent);
        }

        public string Name { get; set; }

        [field: NonSerialized]
        public event Action<Object> log;

        [NonSerialized]
        ICardHandler handler;
        public ICardHandler Handler { get { return handler; } }

        public virtual byte[] ATR
        {
            get { return atr; }
            set { atr = value; }
        }
        protected byte[] atr;
        protected MF masterFile;
        public MF MasterFile { get { return masterFile; } }

        public void SetMF(MF mf) {
            masterFile = mf;
        }

        public Card()
        {
            atr = new byte[] { 0x3b, 0x80, 0x80, 0x01, 0x01 };
            CreateMasterFile();
            handler = CreateHandler(this);
        }

        public virtual ICardHandler CreateHandler(ICard card) {
            return new CardHandler(card);
        }

        public virtual void NewCard() {
            CreateMasterFile();
            handler = CreateHandler(this);
        }

        public virtual void CreateMasterFile()
        {
            masterFile = new MF(this);
        }

        public virtual void ObjectChanged(ICardObject obj, ChangeType type)
        {
            if (objectChanged != null)
                objectChanged(this, obj, type);
        }

        public void Log(object logMsg) {
            if (log != null)
                log(logMsg);
        }

        public void Log(byte[] logMsg)
        {
            if (log != null)
                log(ByteArray.hexDump(logMsg));
        }

        public static byte[] FullPath(ICardObject obj)
        {
            ByteArray path = "";
            while (obj != null)
            {
                path = new ByteArray(new byte[] { Util.UpperByte(obj.ID), Util.LowerByte(obj.ID) }).Append(path);
                obj = obj.Parent;
            }
            return path;
        }

        [field: NonSerialized]
        public event Action<ICard, ICardObject, ChangeType> objectChanged;

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            handler = CreateHandler(this);
        }

        #endregion
    }
    public class ISO7816Exception : Exception {
        public Error error;
        public Error CardError { get { return error; } }
        public ISO7816Exception(Error error) {
            this.error = error;
        }
    }
}
