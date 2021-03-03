using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class ManageSecurityEnvironment : ICardCommand
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
            if (apdu.P1 != 0xf3 &&
                apdu.P1 != 0xf1)
                return Error.P1OrP2NotValid;
            if (apdu.P1 == 0xf3) {
                var se = context.CurDF.GetChildSE(apdu.P2, true);
                if (se == null)
                    return Error.RecordNotFound;
                if (!handler.IsVerifiedAC(se, SE_AC.AC_RESTORE))
                    return Error.SecurityStatusNotSatisfied;
                handler.RestoreSE(se);
            }
            else if (apdu.P1 == 0xf1)
            {
                TLV se=new TLV(apdu.Data);
                byte[] id=se[0x83];
                if (id==null)
                    id=se[0x84];
                if (id==null)
                    return Error.DataFieldNotValid;
                if (id.Length!=1)
                    return Error.DataFieldNotValid;
                SecurityEnvironmentComponent comp = 0;
                switch (apdu.P2) { 
                    case 0xb8:
                        comp = SecurityEnvironmentComponent.CON;
                        break;
                    case 0xa4:
                        comp = SecurityEnvironmentComponent.TEST;
                        break;
                    case 0xb6:
                        comp = SecurityEnvironmentComponent.CDS;
                        break;
                    default:
                        return Error.P1OrP2NotValid;
                }
                if (comp == 0)
                    return Error.P1OrP2NotValid;
                handler.RestoreSE(comp,id[0]);
            }
            return Error.Ok;
        }
    }
}
