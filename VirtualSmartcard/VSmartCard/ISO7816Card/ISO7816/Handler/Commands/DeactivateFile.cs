using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class DeactivateFile : ICardCommand
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
            if (apdu.Data != null && apdu.Data.Length != 0)
                return Error.DataFieldNotValid;

            var ef = context.CurFile;
            if (ef == null)
                return Error.NoCurrentEFSelected;

            if (!ef.Active)
                return Error.InsNotValid;
            ef.Active = false;

            return Error.Ok;
        }
    }
}
