using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class ChangeReferenceData : ICardCommand
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

            encIn = handler.getSMKey(bso, BSO_SM.SM_ENC_CHANGE);
            sigIn = handler.getSMKey(bso, BSO_SM.SM_SIG_CHANGE);
        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;
            if (apdu.P1 != 0 && apdu.P1 != 1) return Error.P1OrP2NotValid;
            bool verify = apdu.P1 == 0;

            ushort pinId = (ushort)(apdu.P2 & 0x7f);
            bool backTrack = (apdu.P2 & 0x80) != 0;
            BSO pin = null;
            if (backTrack)
                pin = context.CurDF.GetChildBSO(pinId, backTrack);
            else
                pin = context.CurDF.Owner.MasterFile.GetChildBSO(pinId, false);
            if (pin == null) return Error.FileNotFound;
            // deve essere un test object di tipo PIN
            if (pin.Class != BSOClass.Test || pin.Algo != BSOAlgo.PIN)
                return Error.InsNotValid;

            int startPin = 0;
            if (verify)
            {
                // cerco il BSO referenziato dall' AC_CHANGE del PIN
                byte condition = pin.AC[BSO_AC.AC_CHANGE];
                if (condition == AC.Never)
                    return Error.ObjectNotFound;
                if (condition != AC.Always)
                {
                    BSO changePin = pin.Parent.GetChildBSO(condition, true);
                    if (changePin == null) return Error.FileNotFound;

                    if (changePin.Class != BSOClass.Test || changePin.Algo != BSOAlgo.PIN)
                        return Error.InsNotValid;

                    if (!handler.VerifyBSO(changePin, new ByteArray(apdu.Data).Left(changePin.Data.Length)))
                        return Error.VerificationFailed;
                    startPin = changePin.Data.Length;
                }
            }
            if (startPin == apdu.Data.Length)
                return Error.WrongLength;
            if (handler.IsVerifiedAC(pin, BSO_AC.AC_CHANGE))
                pin.Data = new ByteArray(apdu.Data).Sub(startPin);
            else
                return Error.SecurityStatusNotSatisfied;

            handler.UnblockBSO(pin);
            return Error.Ok;
        }
    }
}
