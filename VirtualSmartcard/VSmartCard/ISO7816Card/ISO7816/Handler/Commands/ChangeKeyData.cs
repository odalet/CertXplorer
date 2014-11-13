using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class ChangeKeyData : ICardCommand
    {
        IISO7816Card card;
        public IISO7816Card Card { set { card = value; } }
        CardHandler handler;
        public CardHandler Handler { set { handler = value; } }

        public void getSMKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut)
        {
            CardContext context = handler.Context;

            sigOut = null;
            encOut = null;

            ushort bsoId = Util.ToUShort(apdu.P1, (byte)(apdu.P2 & 0x7f));
            bool backTrack = (apdu.P2 & 0x80) != 0;
            BSO bso = null;
            if (backTrack)
                bso = context.CurDF.GetChildBSO(bsoId, backTrack);
            else
                bso = context.CurDF.Owner.MasterFile.GetChildBSO(bsoId, false);
            if (bso == null) 
                throw new ISO7816Exception(Error.FileNotFound);

            encIn = handler.getSMKey(bso, BSO_SM.SM_ENC_CHANGE);
            sigIn = handler.getSMKey(bso, BSO_SM.SM_SIG_CHANGE);
        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;

            ushort bsoId = Util.ToUShort(apdu.P1, (byte)(apdu.P2 & 0x7f));
            bool backTrack = (apdu.P2 & 0x80) != 0;
            BSO bso = null;
            if (backTrack)
                bso = context.CurDF.GetChildBSO(bsoId, backTrack);
            else
                bso = context.CurDF.Owner.MasterFile.GetChildBSO(bsoId, false);
            if (bso == null) return Error.FileNotFound;

            if (!handler.IsVerifiedAC(bso, BSO_AC.AC_CHANGE))
                return Error.SecurityStatusNotSatisfied;

            if (bso.Data.Length != apdu.Data.Length)
                return Error.DataFieldNotValid;
            bso.Data = apdu.Data;

            handler.UnblockBSO(bso);
            return Error.Ok;
        }
    }
}
