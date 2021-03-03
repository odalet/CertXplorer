using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    public class GenerateKeyPair : ICardCommand
    {
        IISO7816Card card;
        public IISO7816Card Card { set { card = value; } }
        CardHandler handler;
        public CardHandler Handler { set { handler = value; } }

        public void getSMKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut)
        {
            sigIn = null;
            encIn = null;
            sigOut = null;
            encOut = null;
        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;
            if (apdu.P1 != 0 || apdu.P2 != 0) 
                return Error.P1OrP2NotValid;
            if (apdu.Data == null || apdu.Data.Length != 8)
                return Error.DataFieldNotValid;

            ByteArray data = apdu.Data;
            if (data[4]!=0 || data[5]!=0)
                return Error.DataFieldNotValid;

            ushort pubExpLen=Util.ToUShort(data,6);
            if (pubExpLen == 0) pubExpLen = 24;
            if (pubExpLen<16 || pubExpLen>64)
                return Error.DataFieldNotValid;

            var Module = context.CurDF.GetChildBSO(Util.ToUShort(data[0],data[1]), false);
            if (Module == null)
                return Error.ObjectNotFound;
            if (Module.Algo != BSOAlgo.RSA_DS_Test &&
                Module.Algo != BSOAlgo.RSA_Enc)
                return Error.InsNotValid;

            if (!handler.IsVerifiedAC(Module, BSO_AC.AC_GENKEYPAIR))
                return Error.SecurityStatusNotSatisfied;

            var privExp = context.CurDF.GetChildBSO(Util.ToUShort((byte)(data[0] | 1), data[1]), false);
            if (privExp == null)
                return Error.ObjectNotFound;
            if (privExp.Algo != BSOAlgo.RSA_DS_Test &&
                privExp.Algo != BSOAlgo.RSA_Enc)
                return Error.InsNotValid;

            var pubKey = context.CurDF.GetChildEF(Util.ToUShort(data.Sub(2, 2)));
            if (pubKey == null)
                return Error.FileNotFound;

            if (!(pubKey is EFLinearTLV))
                return Error.CommandIncompatibleWithFileStructure;

            if (!handler.IsVerifiedAC(pubKey, EF_AC.AC_APPEND))
                return Error.SecurityStatusNotSatisfied;
            
            handler.GenerateKey(privExp, Module, pubKey as EFLinearTLV, pubExpLen);

            return Error.Ok;
        }
    }
}
