using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;
using VirtualSmartCard;

namespace ISO7816.Handler.Commands
{
    class UpdateRecord : ICardCommand
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

            var efBin = context.CurEF as EFRecord;
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

            if (!(context.CurEF is EFRecord))
                return Error.CommandIncompatibleWithFileStructure;

            if (apdu.Data == null || apdu.Data.Length == 0)
                return Error.DataFieldNotValid;

            var efRec = context.CurEF as EFRecord;

            if (!handler.IsVerifiedAC(efRec, EF_AC.AC_UPDATE))
                return Error.SecurityStatusNotSatisfied;

            int? recNum=0;
            if (apdu.P1 == 0) {
                if (apdu.P2 == 0)
                    recNum = 0;
                else if (apdu.P2 == 1)
                    recNum = efRec.Data.Count - 1;
                else if (apdu.P2 == 2)
                {
                    if (!context.CurRecord.HasValue)
                        recNum = 0;
                    else
                        recNum = context.CurRecord.Value+1;
                }
                else if (apdu.P2 == 3)
                {
                    if (!context.CurRecord.HasValue)
                        recNum = efRec.Data.Count - 1;
                    else
                        recNum = context.CurRecord.Value - 1;
                }
                else if (apdu.P2 == 4)
                {
                    if (!context.CurRecord.HasValue)
                        recNum = 0;
                    else
                        recNum = context.CurRecord.Value;
                }
            }
            else if (apdu.P2 == 4) {
                if (apdu.P1 <= efRec.Data.Count && apdu.P1 > 0)
                    recNum = apdu.P1 - 1;
                else
                    return Error.RecordNotFound;
            }
            else if (efRec is EFLinearTLV) {
                var efl = efRec as EFLinearTLV;
                if (apdu.P2 == 0) {
                    recNum = efl.SearchRecord(apdu.P1);
                }
                else if (apdu.P2 == 1) {
                    recNum = efl.SearchRecordRev(apdu.P1);
                }
                else if (apdu.P2 == 2) {
                    if (context.CurRecord.HasValue)
                        recNum = efl.SearchRecord(context.CurRecord.Value, apdu.P1);
                    else
                        recNum = efl.SearchRecord(apdu.P1);
                }
                else if (apdu.P2 == 3) {
                    if (context.CurRecord.HasValue)
                        recNum = efl.SearchRecordRev(context.CurRecord.Value, apdu.P1);
                    else
                        recNum = efl.SearchRecordRev(apdu.P1);
                }
            }
            if (!recNum.HasValue)
                return Error.RecordNotFound;

            if (apdu.P2 != 4)
                if (!(efRec is EFCyclic))
                    context.CurRecord = recNum;
                else
                    context.CurRecord = 0;

            if (recNum.Value >= efRec.Data.Count)
                return Error.RecordNotFound;

            efRec.Update(recNum.Value, apdu.Data);
            return Error.Ok;
        }
    }
}
