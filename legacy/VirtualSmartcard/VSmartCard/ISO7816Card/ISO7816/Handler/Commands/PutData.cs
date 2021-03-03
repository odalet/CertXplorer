using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    class PutData : ICardCommand
    {
        IISO7816Card card;
        public IISO7816Card Card { set { card = value; } }
        CardHandler handler;
        public CardHandler Handler { set { handler = value; } }
        CardContext context;

        public void PutDataSEKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut) {
            sigOut = null;
            encOut = null;

            if (context.CurDF == null)
                throw new ISO7816Exception(Error.ClaNotValid);

            encIn = handler.getSMKey(context.CurDF, DF_SM.SM_ENC_UPDATE_APPEND);
            sigIn = handler.getSMKey(context.CurDF, DF_SM.SM_SIG_UPDATE_APPEND);
        }

        public void PutDataFCIKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut)
        {
            if (context.CurFile == null)
                throw new ISO7816Exception(Error.NoCurrentEFSelected);

            sigOut = null;
            encOut = null;

            if (context.CurFile is DF)
            {
                encIn = handler.getSMKey(context.CurFile, DF_SM.SM_ENC_ADMIN);
                sigIn = handler.getSMKey(context.CurFile, DF_SM.SM_SIG_ADMIN);
            }
            else if (context.CurFile is EF)
            {
                encIn = handler.getSMKey(context.CurFile, EF_SM.SM_ENC_ADMIN);
                sigIn = handler.getSMKey(context.CurFile, EF_SM.SM_SIG_ADMIN);
            }
            else
                throw new ISO7816Exception(Error.NoCurrentEFSelected);
        }

        public void PutDataOCIKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut)
        {
            sigOut = null;
            encOut = null;

            if (context.CurDF == null)
                throw new ISO7816Exception(Error.ClaNotValid);

            encIn = handler.getSMKey(context.CurDF, DF_SM.SM_ENC_UPDATE_APPEND);
            sigIn = handler.getSMKey(context.CurDF, DF_SM.SM_SIG_UPDATE_APPEND);
        }

        public void getSMKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut)
        {
            context = handler.Context;

            if (apdu.P1 == 0x01 && apdu.P2 == 0x6E)
                PutDataOCIKeys(apdu, out sigIn, out encIn, out sigOut, out encOut);
            else if (apdu.P1 == 0x01 && apdu.P2 == 0x6F)
                PutDataFCIKeys(apdu, out sigIn, out encIn, out sigOut, out encOut);
            else if (apdu.P1 == 0x01 && apdu.P2 == 0x6D)
                PutDataSEKeys(apdu, out sigIn, out encIn, out sigOut, out encOut);
            else throw new ISO7816Exception(Error.P1OrP2NotValid);
        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            context = handler.Context;
            if (apdu.P1 == 0x01 && apdu.P2 == 0x6E)
                return PutDataOCI(apdu);
            if (apdu.P1 == 0x01 && apdu.P2 == 0x6F)
                return PutDataFCI(apdu);
            if (apdu.P1 == 0x01 && apdu.P2 == 0x6D)
                return PutDataSE(apdu);

            return Error.P1OrP2NotValid;
        }

        public virtual byte[] PutDataFCI(Apdu apdu)
        {
            TLV fci = new TLV(apdu.Data);
            var Aid = fci[0x84];
            var AC = fci[0x86];
            var SM = card.GetSMTLV(fci);

            if (Aid!=null && Aid.Length > 16) 
                return Error.DataFieldNotValid;

            if (context.CurFile is EF)
            {
                if (!handler.IsVerifiedAC(context.CurFile, EF_AC.AC_ADMIN))
                    return Error.SecurityStatusNotSatisfied;
            }
            else if (context.CurFile is DF)
            {
                if (!handler.IsVerifiedAC(context.CurFile, DF_AC.AC_ADMIN))
                    return Error.SecurityStatusNotSatisfied;
            }

            if (context.CurEF != null && Aid != null) 
                return Error.CommandIncompatibleWithFileStructure;
            if (AC != null) context.CurFile.AC.Set(AC);
            if (SM != null) context.CurFile.SM.Set(SM);
            
            if (context.CurDF != null)
                if (Aid != null) context.CurDF.AID = Aid;

            return Error.Ok;
        }

        public virtual byte[] PutDataSE(Apdu apdu) {
            TLV seci = new TLV(apdu.Data);
            var Id = seci[0x83];
            if (context.CurDF.GetChildSE(Id[0]) != null)
                return UpdateSE(seci);
            else
                return CreateSE(seci);
        }
        byte[] UpdateSE(TLV seci)
        {
            var Id = seci[0x83];
            var AC = seci[0x86];

            if (Id == null) return Error.DataFieldNotValid;
            if (AC == null) return Error.DataFieldNotValid;

            if (Id.Length != 1) return Error.DataFieldNotValid;

            if (context.CurDF == null)
                return Error.ClaNotValid;

            if (!handler.IsVerifiedAC(context.CurDF, DF_AC.AC_UPDATE))
                return Error.SecurityStatusNotSatisfied;

            var se = context.CurDF.GetChildSE(Id[0]);
            se.AC.Set(AC);

            return Error.Ok;
        }

        byte[] CreateSE(TLV seci)
        {
            var Id = seci[0x83];
            var Data = seci[0x8F];
            var AC = seci[0x86];

            if (Id == null) return Error.DataFieldNotValid;
            if (Data == null) return Error.DataFieldNotValid;
            if (AC == null) return Error.DataFieldNotValid;

            if (Id.Length != 1) return Error.DataFieldNotValid;
            if (Data.Length != 6) return Error.DataFieldNotValid;

            if (context.CurDF == null)
                return Error.ClaNotValid;

            if (!handler.IsVerifiedAC(context.CurDF, DF_AC.AC_APPEND))
                return Error.SecurityStatusNotSatisfied;

            var se = card.CreateSE(Id[0], context.CurDF);
            se.AC.Set(AC);
            se.Data = Data;

            return Error.Ok;
        }

        public virtual byte[] PutDataOCI(Apdu apdu)
        {
            TLV oci = new TLV(apdu.Data);
            ushort id = Util.ToUShort(oci[0x83]);
            if (context.CurDF == null)
                return Error.ClaNotValid;
            var obj = context.CurDF.GetChildBSO(id);
            if (obj == null)
                return CreateBSO(oci);
            else
            {
                if (!(obj is BSO))
                    return Error.DataFieldNotValid;
                return UpdateBSO(obj as BSO, oci);
            }
        }
        byte[] UpdateBSO(BSO bso, TLV oci)
        {
            var AC = oci[0x86];
            var SM = card.GetSMTLV(oci);
            var Data = oci[0x8F];

            var curDF = context.CurDF;
            if (curDF == null)
                return Error.InsNotValid;

            if (!handler.IsVerifiedAC(context.CurDF, DF_AC.AC_UPDATE))
                return Error.SecurityStatusNotSatisfied;

            if (AC != null) bso.AC.Set(AC);
            if (SM != null) bso.SM.Set(SM);
            if (Data != null) bso.Data = Data;

            return Error.Ok;
        }

        byte[] CreateBSO(TLV oci)
        {
            var Id = oci[0x83];
            var Options = oci[0x85];
            var AC = oci[0x86];
            var SM = card.GetSMTLV(oci);
            var Data = oci[0x8F];

            if (Id == null) return Error.DataFieldNotValid;
            if (Options == null) return Error.DataFieldNotValid;
            if (Data == null) return Error.DataFieldNotValid;
            if (AC == null) return Error.DataFieldNotValid;

            if (Options.Length != 8) return Error.DataFieldNotValid;
            if (Id.Length != 2) return Error.DataFieldNotValid;

            if (!card.CheckBSOId(Id[1]))
                return Error.DataFieldNotValid;

            var curDF = context.CurDF;
            if (curDF == null)
                return Error.InsNotValid;

            if (!handler.IsVerifiedAC(context.CurDF, DF_AC.AC_APPEND))
                return Error.SecurityStatusNotSatisfied;

            //if (!curDF->ACGranted(AC_DF_APPEND))
            //    SCReturnWithError(0x6982);

            BSO bso = new BSO(Util.ToUShort(Id), card, curDF);
            bso.Options = Options;
            bso.AC.Set(AC);
            if (SM != null)
                bso.SM.Set(SM);
            bso.Data = Data;

            return Error.Ok;
        }
    }
}
