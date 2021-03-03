using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISO7816.FileSystem
{
    public static class SE_AC
    {
        public const byte AC_RESTORE = 0x00;
    }
    public static class DF_AC
    {
        public const byte AC_UPDATE = 0x01;
        public const byte AC_APPEND = 0x02;
        public const byte AC_ADMIN = 0x06;
        public const byte AC_CREATE = 0x07;
    }
    public static class EF_AC
    {
        public const byte AC_READ = 0x00;
        public const byte AC_UPDATE = 0x01;
        public const byte AC_APPEND = 0x02;
        public const byte AC_ADMIN = 0x06;
    }
    public static class BSO_AC
    {
        public const byte AC_USE = 0x00;
        public const byte AC_CHANGE = 0x01;
        public const byte AC_UNBLOCK = 0x02;
        public const byte AC_GENKEYPAIR = 0x06;
    }
    [Serializable]
    public class AC
    {
        public void Set(byte[] ac) { 
            if (ac.Length>numAC)
                throw new Exception("Max number of ACs is " + numAC);
            conditions = new byte[numAC];
            for (int i = 0; i < numAC; i++)
                conditions[i] = Never;
            ac.CopyTo(conditions, 0);
        }
        public override string ToString()
        {
            string str="";
            for (byte i = 0; i < numAC; i++)
                str += this[i].ToString("X02") + " ";
            return str;
        }
        int numAC;
        public int NumAC { get { return numAC; } }
        public const byte Always = 0;
        public const byte Never = 0xff;
        byte[] conditions;
        public byte[] Conditions { get { return conditions; } }
        IObjectWithAC owner;
        public AC(byte[] conditions, IObjectWithAC owner, int numAC)
        {
            this.numAC = numAC;
            this.owner = owner;
            if (conditions.Length > numAC)
                throw new ISO7816Exception(Error.DataFieldNotValid);
            this.conditions = conditions;
        }
        public AC(byte allConditions, IObjectWithAC owner,int numAC)
        {
            this.numAC = numAC;
            this.owner = owner;
            conditions = new byte[numAC];
            for (int i = 0; i < numAC; i++)
                conditions[i] = allConditions;
        }
        public byte this[byte operation]
        {
            get {
                if (conditions == null || operation>=conditions.Length)
                    return AC.Never;
                return conditions[operation]; 
            }
            set {
                if (operation >= numAC)
                    throw new Exception("Max number of ACs is " + numAC);
                if (conditions == null || operation >= conditions.Length) {
                    var newConditions = new byte[operation+1];
                    for (int i = 0; i <= operation; i++)
                        newConditions[i] = AC.Never;
                        if (conditions != null)
                            conditions.CopyTo(newConditions, 0);
                }
                conditions[operation] = value;
            }
        }
    }
}
