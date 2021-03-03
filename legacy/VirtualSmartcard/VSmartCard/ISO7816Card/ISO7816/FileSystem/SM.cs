using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISO7816.FileSystem
{
    public static class DF_SM
    {
        public const byte SM_ENC_UPDATE_APPEND = 0x02;
        public const byte SM_SIG_UPDATE_APPEND = 0x03;
        public const byte SM_ENC_ADMIN = 12;
        public const byte SM_SIG_ADMIN = 13;
        public const byte SM_ENC_CREATE = 14;
        public const byte SM_SIG_CREATE = 15;
    }
    public static class EF_SM
    {
        public const byte SM_ENC_READ_OUT = 0x00;
        public const byte SM_SIG_READ_OUT = 0x01;
        public const byte SM_ENC_UPDATE = 0x02;
        public const byte SM_SIG_UPDATE = 0x03;
        public const byte SM_ENC_APPEND = 0x04;
        public const byte SM_SIG_APPEND = 0x05;
        public const byte SM_ENC_ADMIN = 12;
        public const byte SM_SIG_ADMIN = 13;
        public const byte SM_ENC_READ_IN = 22;
        public const byte SM_SIG_READ_IN = 23;
    }
    public static class BSO_SM
    {
        public const byte SM_ENC_USE = 0x00;
        public const byte SM_SIG_USE = 0x01;
        public const byte SM_ENC_CHANGE = 0x02;
        public const byte SM_SIG_CHANGE = 0x03;
        public const byte SM_ENC_UNBLOCK = 0x04;
        public const byte SM_SIG_UNBLOCK = 0x05;
        public const byte SM_ENC_USE_OUT = 14;
        public const byte SM_SIG_USE_OUT = 15;
    }
    [Serializable]
    public class SM
    {
        public void Set(byte[] sm)
        {
            if (sm.Length > numSM)
                throw new Exception("Max number of SMs is " + numSM);
            keys = new byte[numSM];
            for (int i = 0; i < numSM; i++)
                keys[i] = NoSM;
            sm.CopyTo(keys, 0);
        }

        public override string ToString()
        {
            string str = "";
            for (byte i = 0; i < numSM; i++)
                str += this[i].ToString("X02") + " ";
            return str;
        }

        public const byte NoSM = 0xff;
        byte[] keys;
        int numSM;
        public int NumSM { get { return numSM; } }
        public byte[] Keys { get { return keys; } }
        IObjectWithSM owner;
        public SM(byte[] keys, IObjectWithSM owner,int numSM)
        {
            this.numSM = numSM;
            this.owner = owner;
            if (keys.Length > numSM)
                throw new ISO7816Exception(Error.DataFieldNotValid);
            foreach (byte b in keys)
                if (b == 0)
                    throw new ISO7816Exception(Error.DataFieldNotValid);
            this.keys = keys;
        }
        public SM(byte allKeyEnc, byte allKeySig, IObjectWithSM owner, int numSM)
        {
            this.numSM = numSM;
            this.owner = owner;
            keys = new byte[numSM];
            for (int i = 0; i < numSM; i++)
            {
                if ((i%2)==0) 
                    keys[i] = allKeyEnc;
                else
                    keys[i] = allKeySig;
            }
        }
        public byte this[byte operation]
        {
            get
            {
                if (keys == null || operation >= keys.Length)
                    return AC.Never;
                return keys[operation];
            }
            set
            {
                if (value==0)
                    throw new Exception("SM value not valid");
                if (operation >= numSM)
                    throw new Exception("Max number of SMs is " + numSM);
                if (keys == null || operation >= keys.Length)
                {
                    var newKeys = new byte[operation + 1];
                    for (int i = 0; i <= operation; i++)
                        newKeys[i] = SM.NoSM;
                    if (keys != null)
                        keys.CopyTo(newKeys, 0);
                }
                keys[operation] = value;
            }
        }
    }
}
