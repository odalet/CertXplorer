using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;
using ISO7816;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace ISO7816Card
{
    public class BinEdit : UITypeEditor {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
			var blobform = new BlobForm(ByteArray.parseHex(value.ToString()));
			if (blobform.ShowDialog() == DialogResult.OK)
				return ByteArray.hexDump(blobform.Data);
			throw new OperationCanceledException();
        }
    }
    public class ListEdit<T> : UITypeEditor where T : Typed<byte>
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            ListBox lb = new ListBox();
            lb.SelectionMode = SelectionMode.One;

            foreach (KeyValuePair<byte,string> v in (typeof(T).GetField("values", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null)) as IEnumerable)
            {
                lb.Items.Add(v.Key.ToString("X02")+":"+v.Value);
            }

            lb.Click+=new EventHandler((a,b)=>{
                _editorService.CloseDropDown();
            });
            _editorService.DropDownControl(lb);
            if (lb.SelectedItem != null)
                return lb.SelectedItem.ToString().Split(':')[0];

            return "00";
        } 
        public override bool IsDropDownResizable
        {
            get
            {
                return true;
            }
        }
    }
    public interface ICardWrapper {
        ICardObject getCardObject();
        string FullPath { get; }
    }

    public class WrapperSM
    {
        protected IObjectWithSM obj;
        protected string getSM(byte cond)
        {
            return obj.SM[cond].ToString("X02");
        }
        protected void setSM(byte cond, string value)
        {
            obj.SM[cond] = ByteArray.parseHex(value)[0];
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(obj.SM.NumSM * 3);
            for (byte i = 0; i < obj.SM.NumSM; i++)
            { 
                sb.Append(obj.SM[i].ToString("X02"));
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
    public class WrapperAC
    {
        protected IObjectWithAC obj;
        protected string getAC(byte cond)
        {
            return obj.AC[cond].ToString("X02");
        }
        protected void setAC(byte cond,string value)
        {
            obj.AC[cond] = ByteArray.parseHex(value)[0];
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(obj.AC.NumAC * 3);
            for (byte i = 0; i < obj.AC.NumAC; i++)
            { 
                sb.Append(obj.AC[i].ToString("X02"));
                sb.Append(" ");
            }
            return sb.ToString();
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperDFAC : WrapperAC
    {
        //public override string ToString()
        //{
        //    return "AC for " + (obj as ICardObject).Description;
        //}
        public WrapperDFAC(IObjectWithAC obj)
        {
            this.obj = obj;
        }
        [NotifyParentProperty(true)]
        public string UPDATE { get { return getAC(DF_AC.AC_UPDATE); } set { setAC(DF_AC.AC_UPDATE,value); } }
        [NotifyParentProperty(true)]
        public string APPEND { get { return getAC(DF_AC.AC_APPEND); } set { setAC(DF_AC.AC_APPEND, value); } }
        [NotifyParentProperty(true)]
        public string ADMIN { get { return getAC(DF_AC.AC_ADMIN); } set { setAC(DF_AC.AC_ADMIN, value); } }
        [NotifyParentProperty(true)]
        public string CREATE { get { return getAC(DF_AC.AC_CREATE); } set { setAC(DF_AC.AC_CREATE, value); } }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperDFSM : WrapperSM
    {
        //public override string ToString()
        //{
        //    return "SM for " + (obj as ICardObject).Description;
        //}
        public WrapperDFSM(IObjectWithSM obj)
        {
            this.obj = obj;
        }
        [NotifyParentProperty(true)]
        public string ENC_ADMIN { get { return getSM(DF_SM.SM_ENC_ADMIN); } set { setSM(DF_SM.SM_ENC_ADMIN, value); } }
        [NotifyParentProperty(true)]
        public string SIG_ADMIN { get { return getSM(DF_SM.SM_SIG_ADMIN); } set { setSM(DF_SM.SM_SIG_ADMIN, value); } }
        [NotifyParentProperty(true)]
        public string ENC_CREATE { get { return getSM(DF_SM.SM_ENC_CREATE); } set { setSM(DF_SM.SM_ENC_CREATE, value); } }
        [NotifyParentProperty(true)]
        public string SIG_CREATE { get { return getSM(DF_SM.SM_SIG_CREATE); } set { setSM(DF_SM.SM_SIG_CREATE, value); } }
        [NotifyParentProperty(true)]
        public string ENC_UPDATE_APPEND { get { return getSM(DF_SM.SM_ENC_UPDATE_APPEND); } set { setSM(DF_SM.SM_ENC_UPDATE_APPEND, value); } }
        [NotifyParentProperty(true)]
        public string SIG_UPDATE_APPEND { get { return getSM(DF_SM.SM_SIG_UPDATE_APPEND); } set { setSM(DF_SM.SM_SIG_UPDATE_APPEND, value); } }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperEFAC : WrapperAC
    {
        //public override string ToString()
        //{
        //    return "AC for " + (obj as ICardObject).Description;
        //}
        public WrapperEFAC(IObjectWithAC obj)
        {
            this.obj = obj;
        }
        [NotifyParentProperty(true)]
        public string UPDATE { get { return getAC(EF_AC.AC_UPDATE); } set { setAC(EF_AC.AC_UPDATE, value); } }
        [NotifyParentProperty(true)]
        public string APPEND { get { return getAC(EF_AC.AC_APPEND); } set { setAC(EF_AC.AC_APPEND, value); } }
        [NotifyParentProperty(true)]
        public string ADMIN { get { return getAC(EF_AC.AC_ADMIN); } set { setAC(EF_AC.AC_ADMIN, value); } }
        [NotifyParentProperty(true)]
        public string READ { get { return getAC(EF_AC.AC_READ); } set { setAC(EF_AC.AC_READ, value); } }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperEFSM : WrapperSM
    {
        //public override string ToString()
        //{
        //    return "SM for " + (obj as ICardObject).Description;
        //}
        public WrapperEFSM(IObjectWithSM obj)
        {
            this.obj = obj;
        }
        [NotifyParentProperty(true)]
        public string ENC_ADMIN { get { return getSM(EF_SM.SM_ENC_ADMIN); } set { setSM(EF_SM.SM_ENC_ADMIN, value); } }
        [NotifyParentProperty(true)]
        public string SIG_ADMIN { get { return getSM(EF_SM.SM_SIG_ADMIN); } set { setSM(EF_SM.SM_SIG_ADMIN, value); } }
        [NotifyParentProperty(true)]
        public string ENC_APPEND { get { return getSM(EF_SM.SM_ENC_APPEND); } set { setSM(EF_SM.SM_ENC_APPEND, value); } }
        [NotifyParentProperty(true)]
        public string SIG_APPEND { get { return getSM(EF_SM.SM_SIG_APPEND); } set { setSM(EF_SM.SM_SIG_APPEND, value); } }
        [NotifyParentProperty(true)]
        public string ENC_UPDATE { get { return getSM(EF_SM.SM_ENC_UPDATE); } set { setSM(EF_SM.SM_ENC_UPDATE, value); } }
        [NotifyParentProperty(true)]
        public string SIG_UPDATE { get { return getSM(EF_SM.SM_SIG_UPDATE); } set { setSM(EF_SM.SM_SIG_UPDATE, value); } }
        [NotifyParentProperty(true)]
        public string ENC_READ_IN { get { return getSM(EF_SM.SM_ENC_READ_IN); } set { setSM(EF_SM.SM_ENC_READ_IN, value); } }
        [NotifyParentProperty(true)]
        public string SIG_READ_IN { get { return getSM(EF_SM.SM_SIG_READ_IN); } set { setSM(EF_SM.SM_SIG_READ_IN, value); } }
        [NotifyParentProperty(true)]
        public string ENC_READ_OUT { get { return getSM(EF_SM.SM_ENC_READ_OUT); } set { setSM(EF_SM.SM_ENC_READ_OUT, value); } }
        [NotifyParentProperty(true)]
        public string SIG_READ_OUT { get { return getSM(EF_SM.SM_SIG_READ_OUT); } set { setSM(EF_SM.SM_SIG_READ_OUT, value); } }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperBSOAC : WrapperAC
    {
        //public override string ToString()
        //{
        //    return "AC for " + (obj as ICardObject).Description;
        //}
        public WrapperBSOAC(IObjectWithAC obj)
        {
            this.obj = obj;
        }
        [NotifyParentProperty(true)]
        public string USE { get { return getAC(BSO_AC.AC_USE); } set { setAC(BSO_AC.AC_USE, value); } }
        [NotifyParentProperty(true)]
        public string CHANGE { get { return getAC(BSO_AC.AC_CHANGE); } set { setAC(BSO_AC.AC_CHANGE, value); } }
        [NotifyParentProperty(true)]
        public string UNBLOCK { get { return getAC(BSO_AC.AC_UNBLOCK); } set { setAC(BSO_AC.AC_UNBLOCK, value); } }
        [NotifyParentProperty(true)]
        public string GENKEYPAIR { get { return getAC(BSO_AC.AC_GENKEYPAIR); } set { setAC(BSO_AC.AC_GENKEYPAIR, value); } }
    }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperBSOSM : WrapperSM
    {
        //public override string ToString()
        //{
        //    return "SM for " + (obj as ICardObject).Description;
        //}
        public WrapperBSOSM(IObjectWithSM obj)
        {
            this.obj = obj;
        }
        [NotifyParentProperty(true)]
        public string ENC_USE { get { return getSM(BSO_SM.SM_ENC_USE); } set { setSM(BSO_SM.SM_ENC_USE, value); } }
        [NotifyParentProperty(true)]
        public string SIG_USE { get { return getSM(BSO_SM.SM_SIG_USE); } set { setSM(BSO_SM.SM_SIG_USE, value); } }
        [NotifyParentProperty(true)]
        public string ENC_CHANGE { get { return getSM(BSO_SM.SM_ENC_CHANGE); } set { setSM(BSO_SM.SM_ENC_CHANGE, value); } }
        [NotifyParentProperty(true)]
        public string SIG_CHANGE { get { return getSM(BSO_SM.SM_SIG_CHANGE); } set { setSM(BSO_SM.SM_SIG_CHANGE, value); } }
        [NotifyParentProperty(true)]
        public string ENC_UNBLOCK { get { return getSM(BSO_SM.SM_ENC_UNBLOCK); } set { setSM(BSO_SM.SM_ENC_UNBLOCK, value); } }
        [NotifyParentProperty(true)]
        public string SIG_UNBLOCK { get { return getSM(BSO_SM.SM_SIG_UNBLOCK); } set { setSM(BSO_SM.SM_SIG_UNBLOCK, value); } }
        [NotifyParentProperty(true)]
        public string ENC_USE_OUT { get { return getSM(BSO_SM.SM_ENC_USE_OUT); } set { setSM(BSO_SM.SM_ENC_USE_OUT, value); } }
        [NotifyParentProperty(true)]
        public string SIG_USE_OUT { get { return getSM(BSO_SM.SM_SIG_USE_OUT); } set { setSM(BSO_SM.SM_SIG_USE_OUT, value); } }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperSEAC : WrapperAC
    {
        //public override string ToString()
        //{
        //    return "AC for " + (obj as ICardObject).Description;
        //}
        public WrapperSEAC(IObjectWithAC obj)
        {
            this.obj = obj;
        }
        [NotifyParentProperty(true)]
        public string RESTORE { get { return getAC(SE_AC.AC_RESTORE); } set { setAC(SE_AC.AC_RESTORE, value); } }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperMF : WrapperDF {
        public string Class {
            get { return ((MF)obj).Owner.GetType().Name; }
        }
        [Editor(typeof(BinEdit), typeof(System.Drawing.Design.UITypeEditor))] 
        public string ATR
        {
            get { return ((MF)obj).ATR != null ? ByteArray.hexDump(((MF)obj).ATR) : ""; }
            set { (((MF)obj).Owner as Card).ATR = ByteArray.parseHex(value); }
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WrapperDF : ICardWrapper
    {
        public string AID
        {
            get { return obj.AID != null ? ByteArray.hexDump(obj.AID) : ""; }
            set { obj.aid=ByteArray.parseHex(value); }
        }
        public string FullPath { get { return obj.FullPath; } }
        public WrapperDFAC AccessCondition { get; set; }
        public WrapperDFSM SecureMessaging { get; set; }

        public ICardObject getCardObject() { return obj; }
        
        protected DF obj;
        public string ID
        {
            get
            {
                return ByteArray.hexDump(Util.ToByteArray(obj.ID));
            }
            set
            {
                obj.ID = Util.ToUShort(ByteArray.parseHex(value));
            }
        }
        public void setObject(DF df) {
            this.obj = df;
            AccessCondition = new WrapperDFAC(obj as IObjectWithAC);
            SecureMessaging = new WrapperDFSM(obj as IObjectWithSM);
        }
    }

    public class WrapperEFBinary : WrapperEF {
        [Editor(typeof(BinEdit), typeof(System.Drawing.Design.UITypeEditor))] 
        public string Data { 
            get { return new ByteArray((obj as EFBinary).Data).ToString(); }
            set { (obj as EFBinary).data = ByteArray.parseHex(value); }
        }
    }

    public class WrapperEFRecord : WrapperEF
    {
        string records;
        public override void setObject(EF ef) {
            base.setObject(ef);
        }

        public WrapperEFRecord()
        {
        }
        public string NumRecords {
            get {
                return (obj as EFRecord).Data.Count.ToString("X");
            }
            set {
                var data=(obj as EFRecord).Data;
                uint numRec=Util.ToUInt(ByteArray.parseHex(value));
                if (data.Count > numRec) { 
                    var list=new List<byte[]>();
                    for(int i=0;i<numRec;i++) {
                        list.Add(data[i]);
                    }
                    (obj as EFRecord).Data = list;
                }
                else if (data.Count < numRec) {
                    var list = new List<byte[]>();
                    list.AddRange(data);
                    for (int i = data.Count; i < numRec; i++)
                        list.Add(new byte[0]);
                    (obj as EFRecord).Data = list;
                }
            }
        }
        public string RecordSize
        {
            get
            {
                if (obj is IObjectWithRecordSize)
                    return (obj as IObjectWithRecordSize).RecordSize.ToString("X");
                else
                    return null;
            }
            set
            {
                if (obj is IObjectWithRecordSize)
                    (obj as IObjectWithRecordSize).RecordSize = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
        }
    }
    public abstract class WrapperEF : ICardWrapper
    {
        public string FullPath { get { return obj.FullPath; } }
        public WrapperEFAC AccessCondition { get; set; }
        public WrapperEFSM SecureMessaging { get; set; }

        public ICardObject getCardObject() { return obj; }

        protected EF obj;
        public string ID
        {
            get
            {
                return ByteArray.hexDump(Util.ToByteArray(obj.ID));
            }
            set
            {
                obj.ID = Util.ToUShort(ByteArray.parseHex(value));
            }
        }

        public string Size
        {
            get { return obj.Size.ToString("X"); }
            set { obj.Size = uint.Parse(value, System.Globalization.NumberStyles.HexNumber); }
        }
        public virtual void setObject(EF ef)
        {
            this.obj = ef;
            AccessCondition = new WrapperEFAC(obj as IObjectWithAC);
            SecureMessaging = new WrapperEFSM(obj as IObjectWithSM);
        }
    }

    public class WrapperBSO : ICardWrapper
    {
        public string FullPath { get { return obj.FullPath; } }
        public WrapperBSOAC AccessCondition { get; set; }
        public WrapperBSOSM SecureMessaging { get; set; }

        public ICardObject getCardObject() { return obj; }
        BSO obj;
        [Editor(typeof(ListEdit<BSOAlgo>), typeof(UITypeEditor))]
        public string Algorithm
        {
            get { return ((byte)obj.Algo).ToString("X02"); }
            set { obj.Algo = new BSOAlgo(byte.Parse(value, System.Globalization.NumberStyles.HexNumber)); }
        }
        [Editor(typeof(ListEdit<BSOClass>), typeof(UITypeEditor))]
        public string Class
        {
            get { return ((byte)obj.Class).ToString("X02"); }
            set { obj.Class = new BSOClass(byte.Parse(value, System.Globalization.NumberStyles.HexNumber)); }
        }
        public bool Blocked
        {
            get { return obj.Blocked; }
            set { obj.Blocked = value; }
        }
        public string CurrentErrorCounter
        {
            get { return obj.CurErrorCounter.ToString("X02"); }
            set { obj.CurErrorCounter = int.Parse(value,System.Globalization.NumberStyles.HexNumber); }
        }
        public string CurrentValidityCounter
        {
            get { return obj.CurValidityCounter.ToString("X02"); }
            set { obj.CurValidityCounter = int.Parse(value, System.Globalization.NumberStyles.HexNumber); }
        }
        public string MaxErrorCounter
        {
            get { return obj.MaxErrorCounter.ToString("X02"); }
            set { obj.MaxErrorCounter = int.Parse(value, System.Globalization.NumberStyles.HexNumber); }
        }
        public string MaxValidityCounter
        {
            get { return obj.ValidityCounter.ToString("X02"); }
            set { obj.ValidityCounter = int.Parse(value, System.Globalization.NumberStyles.HexNumber); }
        }
        public string MinLength
        {
            get { return obj.MinLength.ToString("X02"); }
            set { obj.MinLength = byte.Parse(value, System.Globalization.NumberStyles.HexNumber); }
        }
        [Editor(typeof(BinEdit), typeof(System.Drawing.Design.UITypeEditor))]
        public string Data
        {
             get { return new ByteArray((obj as BSO).Data).ToString(); } 
             set { (obj as BSO).data=ByteArray.parseHex(value); } 
        }
        
    
        public string ID
        {
            get
            {
                return Util.ToByteArray(obj.ID)[1].ToString("X02");
            }
            set
            {

                obj.KeyID = ByteArray.parseHex(value)[0];
            }
        }
        public void setObject(BSO bso)
        {
            this.obj = bso;
            AccessCondition = new WrapperBSOAC(obj as IObjectWithAC);
            SecureMessaging = new WrapperBSOSM(obj as IObjectWithSM);
        }
    }

    public class WrapperSE : ICardWrapper
    {
        public string FullPath { get { return obj.FullPath; } }
        public WrapperSEAC AccessCondition { get; set; }

        public ICardObject getCardObject() { return obj; }

        SecurityEnvironmenet obj;
        public void setObject(SecurityEnvironmenet se)
        {
            this.obj = se;
            AccessCondition = new WrapperSEAC(obj as IObjectWithAC);
        }

        public string ID
        {
            get
            {
                return Util.ToByteArray(obj.ID)[1].ToString("X02");
            }
            set
            {

                obj.ID = (ushort)(0xff00 | ByteArray.parseHex(value)[0]);
            }
        }

        [Editor(typeof(BinEdit), typeof(System.Drawing.Design.UITypeEditor))] 
        public string Data
        {
            get { return new ByteArray((obj as SecurityEnvironmenet).Data).ToString(); }
            set { (obj as SecurityEnvironmenet).data = ByteArray.parseHex(value); }
        }
    }
}
