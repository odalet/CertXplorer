using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    public class ReadBinary : ICardCommand
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

            var efBin = context.CurEF as EFBinary;
            if (efBin == null)
                return;

            encIn = handler.getSMKey(efBin, EF_SM.SM_ENC_READ_IN);
            sigIn = handler.getSMKey(efBin, EF_SM.SM_SIG_READ_IN);

            encOut = handler.getSMKey(efBin, EF_SM.SM_ENC_READ_OUT);
            sigOut = handler.getSMKey(efBin, EF_SM.SM_SIG_READ_OUT);
        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;
            
            if (context.CurEF == null)
                return Error.NoCurrentEFSelected;

            if (apdu.Data != null && apdu.Data.Length > 0)
                return Error.DataFieldNotValid;

            if (!(context.CurEF is EFBinary))
                return Error.CommandIncompatibleWithFileStructure;

            var efBin=context.CurEF as EFBinary;

            if (!handler.IsVerifiedAC(efBin, EF_AC.AC_READ))
                return Error.SecurityStatusNotSatisfied;

            var offset = (apdu.P1 << 8) | apdu.P2;
            int len = 0;
            if (apdu.UseLE) {
                if (apdu.LE == 0) len = 256;
                else len = apdu.LE;
            }
            if (efBin.Data.Length < offset)
                return Error.P1OrP2NotValid;

            if (efBin.Data.Length < offset + len)
                len = efBin.Data.Length - offset;

            return Util.Response(efBin.Data, offset, len, Error.Ok);
        }
    }
}
