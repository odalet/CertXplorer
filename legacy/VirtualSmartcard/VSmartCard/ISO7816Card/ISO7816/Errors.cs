using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISO7816
{
    public class Error
    {
        static public Error ClaNotValid = new Error(0x6d00);
        static public Error InsNotValid = new Error(0x6e00);
        static public Error BSOBlocked = new Error(0x6983);
        static public Error P1OrP2NotValid = new Error(0x6a86);
        static public Error DataFieldNotValid = new Error(0x6a80);
        static public Error SecurityStatusNotSatisfied = new Error(0x6982);
        static public Error SMDataObjectsIncorrect = new Error(0x6988);
        static public Error SMObjectMissing = new Error(0x6987);
        static public Error FileNotFound = new Error(0x6a82);
        static public Error RecordNotFound = new Error(0x6a83);
        static public Error NotEnoughSpace = new Error(0x6a84);
        static public Error ConditionsOfUseNotSatisfied = new Error(0x6985);
        static public Error RecordInconsistentWithTLVDataStructure = new Error(0x6a85);        
        static public Error ObjectNotFound = new Error(0x6a88);
        static public Error NoCurrentEFSelected = new Error(0x6986);
        static public Error ReferencedDataInvalidated = new Error(0x6984);
        static public Error CommandIncompatibleWithFileStructure = new Error(0x6981);
        static public Error WrongLength = new Error(0x6700);
        static public Error InternalError = new Error(0x6f00);
        static public Error VerificationFailed = new Error(0x6300);
        static public Error FileAlreadyExists = new Error(0x6a89);        
        static public Error Ok = new Error(0x9000);
        
        ushort val;
        Error(ushort val) {
            this.val = val;
        }
        public static implicit operator ushort(Error error)
        {
            return error.val;
        }
        public static implicit operator byte[] (Error error) {
            ushort val = error.val;
            return new byte[2] { (byte)(val >> 8), (byte)(val & 0xff) };
        }
    }
}
