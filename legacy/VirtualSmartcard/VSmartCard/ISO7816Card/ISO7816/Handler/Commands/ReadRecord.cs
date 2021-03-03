using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class ReadRecord : ICardCommand
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

            if (!(context.CurEF is EFRecord))
                return Error.CommandIncompatibleWithFileStructure;

            var efRec = context.CurEF as EFRecord;

            if (!handler.IsVerifiedAC(efRec, EF_AC.AC_READ))
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
                context.CurRecord = recNum;

            if (recNum.Value >= efRec.Data.Count)
                return Error.RecordNotFound;
            var recVal=efRec.Read(recNum.Value);

            if (apdu.UseLE && apdu.LE!=0)
                recVal=new ByteArray(recVal).Left(apdu.LE);

            return Util.Response(recVal, Error.Ok);
        }
    }
}
