using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class ExternalAuthenticate : ICardCommand
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

            ushort pinId = (ushort)(apdu.P2 & 0x7f);
            BSO pin = null;
            if (pinId != 0)
            {
                bool backTrack = (apdu.P2 & 0x80) != 0;
                if (backTrack)
                    pin = context.CurDF.GetChildBSO(pinId, backTrack);
                else
                    pin = context.CurDF.Owner.MasterFile.GetChildBSO(pinId, false);
            }
            else
                pin = handler.GetEnvironmentKey(SecurityEnvironmentComponent.TEST);

            if (pin == null)
                throw new ISO7816Exception(Error.FileNotFound);

            encIn = handler.getSMKey(pin, BSO_SM.SM_ENC_USE);
            sigIn = handler.getSMKey(pin, BSO_SM.SM_SIG_USE);

        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;
            if (apdu.P1 != 0) return Error.P1OrP2NotValid;
            ushort pinId = (ushort)(apdu.P2 & 0x7f);
            BSO pin = null;
            if (pinId != 0)
            {
                bool backTrack = (apdu.P2 & 0x80) != 0;
                if (backTrack)
                    pin = context.CurDF.GetChildBSO(pinId, backTrack);
                else
                    pin = context.CurDF.Owner.MasterFile.GetChildBSO(pinId, false);
            }
            else
                pin = handler.GetEnvironmentKey(SecurityEnvironmentComponent.TEST);

            if (pin == null) 
                return Error.FileNotFound;
            if (pin.Class!=BSOClass.Test || pin.Algo==BSOAlgo.PIN)
                return Error.InsNotValid;

            if (!handler.IsVerifiedAC(pin, BSO_AC.AC_USE))
                return Error.SecurityStatusNotSatisfied;

            if (handler.VerifyBSO(pin,apdu.Data))
                return Error.Ok;
            else
                return Error.VerificationFailed;
        }
    }
}

