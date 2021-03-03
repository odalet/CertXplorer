using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    public class CreateFile : ICardCommand
    {
        protected IISO7816Card card;
        public IISO7816Card Card { set { card = value; } }
        protected CardHandler handler;
        public CardHandler Handler { set { handler = value; } }

        public void getSMKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut)
        {
            var context = handler.Context;

            sigOut = null;
            encOut = null;

            encIn = handler.getSMKey(context.CurDF, DF_SM.SM_ENC_CREATE);
            sigIn = handler.getSMKey(context.CurDF, DF_SM.SM_SIG_CREATE);
        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;
            
            if (context.CurDF == null)
                return Error.ClaNotValid;

            TLV fciExt = new TLV(apdu.Data);
            if (fciExt[0x62] == null) return Error.DataFieldNotValid;
            TLV fci = new TLV(fciExt[0x62]);
            var Size1 = fci[0x80];
            var Size2 = fci[0x81];
            var Options = fci[0x82];
            var Id = fci[0x83];
            var Fixed = fci[0x85];
            var AC = fci[0x86];
            var SM = card.GetSMTLV(fci);
            if (Size1 != null && Size2 != null) return Error.DataFieldNotValid;
            if (Options==null) return Error.DataFieldNotValid;
            if (Id == null) return Error.DataFieldNotValid;
            if (Fixed == null) return Error.DataFieldNotValid;
            if (AC == null) return Error.DataFieldNotValid;

            if (Options.Length != 3) return Error.DataFieldNotValid;
            if (Id.Length != 2) return Error.DataFieldNotValid;
            if (Fixed.Length != 1) return Error.DataFieldNotValid;

            if (Fixed[0] != 1) return Error.DataFieldNotValid;
            if (Options[0] == 0x38 && Size2 == null) return Error.DataFieldNotValid;

            if (!handler.IsVerifiedAC(context.CurDF, DF_AC.AC_CREATE))
                return Error.SecurityStatusNotSatisfied;
            ushort newId=Util.ToUShort(Id);

            if (newId == 0x3F00 || newId == 0x3FFF || newId == 0xFFFF)
                return Error.DataFieldNotValid;

            if (context.CurDF.GetChildEForDF(newId) != null)
                return Error.FileAlreadyExists;

            CardSelectable obj = null;
            if (Options[0] == 0x38)
                obj = card.CreateDF(newId, context.CurDF);
            else if (Options[0] == 0x01)
                obj = card.CreateEF(newId, context.CurDF,Util.ToUInt(Size1));
            else if (Options[0] == 0x02)
                obj = new EFLinearFixed(newId, card, context.CurDF, Util.ToUInt(Size1), Options[2]);
            else if (Options[0] == 0x05)
                obj = card.CreateEFLinearTLV(newId, context.CurDF, Util.ToUInt(Size1));
            else if (Options[0] == 0x06)
                obj = new EFCyclic(newId, card, context.CurDF, Util.ToUInt(Size1), Options[2]);
            else
                return Error.DataFieldNotValid;

            if (AC != null)
                obj.AC.Set(AC);
            if (SM != null)
                obj.SM.Set(SM);

            if (obj != null)
                context.CurFile = obj;

            return Error.Ok;
        }
    }
}
