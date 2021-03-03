using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace ISO7816
{
    public class TLV
    {
        public class TLVelem {
            public TLVelem(byte _tag, byte[] _value) {
                tag = _tag;
                if (_value != null)
                    value = _value;
                else
                    value = new byte[0];
            }
            public byte tag;
            public byte[] value;
        }
        public List<TLVelem> elems = new List<TLVelem> ();


        public TLV() { 
        }
        public TLV(byte[] data) {
            int ptr = 0;
            while (ptr < data.Length) {
                byte tag = data[ptr];
                if (tag == 0)
                    return;
                byte elemLen=data[ptr + 1];
                if (ptr + elemLen + 2 <= data.Length)
                {
                    byte[] elemData = new byte[elemLen];
                    Array.Copy(data, ptr + 2, elemData, 0, elemLen);
                    elems.Add(new TLVelem(tag, elemData));
                    ptr += elemLen + 2;
                }
                else
                    throw new Exception("Errore nellastruttura TLV");
            }
        }

        public byte[] this[byte tag]
        {
            get
            {
                foreach (var v in elems)
                {
                    if (v.tag == tag)
                        return v.value;
                }
                return null;
            }
            set
            {
                foreach (var v in elems)
                {
                    if (v.tag == tag)
                        v.value=value;
                }
                elems.Add(new TLVelem(tag,value));
            }
        }

        public void addTag(byte tag, byte[] value) {
            elems.Add(new TLVelem(tag, value));            
        }

		public byte[] getTag(byte tag)
		{
			foreach(TLVelem elem in elems) {
				if (elem.tag==tag)
					return elem.value;
			}
			return null;
		}
		
		public void addTag(TLVelem elem)
        {
            elems.Add(elem);
        }

        public void removeTag(byte tag)
        {
            for (int i = 0; i < elems.Count; i++)
                if (elems[i].tag == tag)
                {
                    elems.RemoveAt(i);
                    return;
                }
        }

        public byte[] GetBytes()
        { 
            int totBytes=0;
            foreach (TLVelem elem in elems) {
                totBytes += 2 + elem.value.Length;
            }
            byte []bytes = new byte[totBytes];
            int i = 0;
            foreach (TLVelem elem in elems)
            {
                bytes[i] = elem.tag; i++;
                bytes[i] = (byte)elem.value.Length; i++;
                elem.value.CopyTo(bytes, i);
                i += elem.value.Length;
            }
            return bytes;
        }
    }
}
