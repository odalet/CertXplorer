using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class UpdateBinary : ICardCommand
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

            encIn = handler.getSMKey(efBin, EF_SM.SM_ENC_UPDATE);
            sigIn = handler.getSMKey(efBin, EF_SM.SM_SIG_UPDATE);
        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;
            
            if (context.CurEF == null)
                return Error.NoCurrentEFSelected;

            if (!(context.CurEF is EFBinary))
                return Error.CommandIncompatibleWithFileStructure;

            var efBin=context.CurEF as EFBinary;

            if (!handler.IsVerifiedAC(efBin,EF_AC.AC_UPDATE))
                return Error.SecurityStatusNotSatisfied;

            var offset = (apdu.P1 << 8) | apdu.P2;
            if (efBin.Data.Length<offset+apdu.Data.Length)
                return Error.WrongLength;

            if (apdu.Data==null || apdu.Data.Length==0)
                return Error.DataFieldNotValid;

            Array.Copy(apdu.Data, 0, efBin.Data, offset, apdu.Data.Length);

            return Error.Ok;
        }
    }
}
