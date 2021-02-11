using System;
using System.Text;
using System.Collections;

namespace SmartCard.NET
{
    public class TLV
    {
        public class TLVelem {
            public TLVelem(byte _tag, byte[] _value) {
                tag = _tag;
                value = _value;
            }
            public byte tag;
            public byte[] value;
        }
        ArrayList elems = new ArrayList ();

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
