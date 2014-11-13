using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class ResetRetryCounter : ICardCommand
    {
        IISO7816Card card;
        public IISO7816Card Card { set { card = value; } }
        CardHandler handler;
        public CardHandler Handler { set { handler = value; } }

        public void getSMKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut)
        {
            CardContext context = handler.Context;

            sigIn = null;
            encIn = null;
            sigOut = null;
            encOut = null;


            ushort bsoId = (ushort)(apdu.P2 & 0x7f);
            bool backTrack = (apdu.P2 & 0x80) != 0;
            BSO bso = null;
            if (backTrack)
                bso = context.CurDF.GetChildBSO(bsoId, backTrack);
            else
                bso = context.CurDF.Owner.MasterFile.GetChildBSO(bsoId, false);

            if (bso == null) 
                throw new ISO7816Exception(Error.ObjectNotFound);

            encIn = handler.getSMKey(bso, BSO_SM.SM_ENC_UNBLOCK);
            sigIn = handler.getSMKey(bso, BSO_SM.SM_SIG_UNBLOCK);

        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;
            if (apdu.P1 != 0 && 
                apdu.P1 != 1 && 
                apdu.P1 != 3) return Error.P1OrP2NotValid;
            bool setNewPin = apdu.P1 == 0;
            bool verifyPin = apdu.P1 != 3;

            if (apdu.P1 == 3 && (apdu.Data != null && apdu.Data.Length > 0))
                return Error.DataFieldNotValid;

            ushort bsoId = (ushort)(apdu.P2 & 0x7f);
            bool backTrack = (apdu.P2 & 0x80) != 0;
            BSO bso = null;
            if (backTrack)
                bso = context.CurDF.GetChildBSO(bsoId, backTrack);
            else
                bso = context.CurDF.Owner.MasterFile.GetChildBSO(bsoId, false);
            if (bso == null) return Error.FileNotFound;
            if (bso.Class != BSOClass.Test)
                return Error.InsNotValid;

            // deve essere un test object di tipo PIN
            if (setNewPin && bso.Algo != BSOAlgo.PIN)
                return Error.InsNotValid;

            int startPin = 0;
            if (verifyPin)
            {
                // cerco il BSO referenziato dall' AC_CHANGE del PIN
                byte condition = bso.AC[BSO_AC.AC_UNBLOCK];
                if (condition == AC.Always || condition == AC.Never)
                    return Error.ObjectNotFound;
                BSO changePin = bso.Parent.GetChildBSO(condition, true);
                if (changePin == null) return Error.FileNotFound;

                if (changePin.Class != BSOClass.Test || changePin.Algo != BSOAlgo.PIN)
                    return Error.InsNotValid;
                int pinLen=0;
                if (!setNewPin)
                    pinLen = apdu.Data != null ? apdu.Data.Length : 0;
                else
                    pinLen = changePin.Data.Length;
                if (!handler.VerifyBSO(changePin, new ByteArray(apdu.Data).Left(pinLen)))
                    return Error.VerificationFailed;
                startPin = changePin.Data.Length;
            }
            if (!handler.IsVerifiedAC(bso, BSO_AC.AC_UNBLOCK))
                throw new ISO7816Exception(Error.SecurityStatusNotSatisfied);

            handler.UnblockBSO(bso);

            if (setNewPin)
                bso.Data = new ByteArray(apdu.Data).Sub(startPin);
            return Error.Ok;
        }
    }
}
