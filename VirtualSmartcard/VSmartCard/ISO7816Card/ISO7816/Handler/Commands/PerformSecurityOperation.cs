using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    public class PerformSecurityOperation : ICardCommand
    {
        protected IISO7816Card card;
        public IISO7816Card Card { set { card = value; } }
        protected CardHandler handler;
        public CardHandler Handler { set { handler = value; } }

        public void getSMKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut)
        {
            sigOut = null;
            encOut = null;

            BSO key =null;
            if (apdu.P1 == 0x80 && apdu.P2 == 0x86)
                key = handler.GetEnvironmentKey(SecurityEnvironmentComponent.CON);
            else if (apdu.P1 == 0x86 && apdu.P2 == 0x80)
                key = handler.GetEnvironmentKey(SecurityEnvironmentComponent.CON);
            else if (apdu.P1 == 0x9E && apdu.P2 == 0x9A)
                key = handler.GetEnvironmentKey(SecurityEnvironmentComponent.CDS);


            encIn = handler.getSMKey(key, BSO_SM.SM_ENC_USE);
            sigIn = handler.getSMKey(key, BSO_SM.SM_SIG_USE);
            encOut = handler.getSMKey(key, BSO_SM.SM_ENC_USE_OUT);
            sigOut = handler.getSMKey(key, BSO_SM.SM_SIG_USE_OUT);
        }

        public virtual byte[] processCommand(Apdu apdu)
        {
            CardContext context = handler.Context;
            if (apdu.P1 == 0x80 && apdu.P2 == 0x86) { 
                // DEC
                BSO key = handler.GetEnvironmentKey(SecurityEnvironmentComponent.CON);
                if (key.Class != BSOClass.PSO)
                    return Error.P1OrP2NotValid;

                if (key.Algo!=BSOAlgo.RSA_Enc && 
                    key.Algo!=BSOAlgo.DES3_Enc_SMEnc)
                    return Error.P1OrP2NotValid;

                if (!handler.IsVerifiedAC(key, BSO_AC.AC_USE))
                    return Error.SecurityStatusNotSatisfied;

                byte[] data=handler.Decrypt(key, new ByteArray(apdu.Data).Sub(1));
                // devo paddare per arrivare alla lunghezza del modulo
                if (key.Algo == BSOAlgo.RSA_Enc)
                {
                    if (data.Length < (key.data.Length - 2))
                    {
                        var dataPad = new byte[key.data.Length - 2];
                        data.CopyTo(dataPad, dataPad.Length - data.Length);
                        data = dataPad;
                    }
                }
                return Util.Response(data, Error.Ok);
            }
            else if (apdu.P1 == 0x86 && apdu.P2 == 0x80)
            {
                // ENC
                BSO key = handler.GetEnvironmentKey(SecurityEnvironmentComponent.CON);
                if (key.Class != BSOClass.PSO)
                    return Error.P1OrP2NotValid;

                if (key.Algo != BSOAlgo.RSA_Enc &&
                    key.Algo != BSOAlgo.DES3_Enc_SMEnc)
                    return Error.P1OrP2NotValid;

                if (!handler.IsVerifiedAC(key, BSO_AC.AC_USE))
                    return Error.SecurityStatusNotSatisfied;

                byte[] data = handler.Encrypt(key, apdu.Data);
                if (key.Algo == BSOAlgo.RSA_Enc && data.Length < (key.data.Length - 2))
                {
                    var dataPad = new byte[key.data.Length - 2];
                    data.CopyTo(dataPad, dataPad.Length - data.Length);
                    data = dataPad;
                }
                return Util.Response(new ByteArray(0).Append(data), Error.Ok);
            }
            else if (apdu.P1 == 0x9E && apdu.P2 == 0x9A)
            {
                // CDS
                BSO key = handler.GetEnvironmentKey(SecurityEnvironmentComponent.CDS);
                if (key.Class != BSOClass.PSO)
                    return Error.P1OrP2NotValid;

                if (key.Algo != BSOAlgo.RSA_DS_Test)
                    return Error.P1OrP2NotValid;

                if (!handler.IsVerifiedAC(key, BSO_AC.AC_USE))
                    return Error.SecurityStatusNotSatisfied;

                byte[] data = handler.DigitalSignature(key, apdu.Data);
                if (data.Length < (key.data.Length - 2))
                {
                    var dataPad = new byte[key.data.Length - 2];
                    data.CopyTo(dataPad, dataPad.Length - data.Length);
                    data = dataPad;
                }
                return Util.Response(data, Error.Ok);
            }
            return Error.P1OrP2NotValid;
        }
    }
}
