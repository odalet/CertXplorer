using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;
using ISO7816.Handler.Commands;
using System.Security.Cryptography;
using System.IO;
using VirtualSmartCard;
using System.Reflection;

namespace ISO7816.Handler
{
    public interface ICardCommand {
        IISO7816Card Card { set; }
        CardHandler Handler { set; }
        byte[] processCommand(Apdu apdu);
        void getSMKeys(Apdu apdu, out BSO sigIn, out BSO encIn, out BSO sigOut, out BSO encOut);
    }
    public class CardContext
    {
        public byte[] Challenge { get; set; }
        public byte[] Random { get; set; }
        public DF CurrentSecurityEntironmentDF { get; set; }
        public Dictionary<SecurityEnvironmentComponent, BSO> SEcomponents { get;set; }

        public int? CurRecord { get; set; }
        public DF CurDF {get {
            if (curFile==null)
                return null;
            if (curFile is DF)
                return curFile as DF;
            return curFile.Parent;
        }}
        public EF CurEF {get {
            if (curFile==null)
                return null;
            if (curFile is EF)
                return curFile as EF;
            return null;
        }}
        CardSelectable curFile = null;
        public CardSelectable CurFile
        {
            get { return curFile; }
            set {
                if (curFile != null)
                    curFile.Owner.ObjectChanged(curFile, ChangeType.Unselected);
                curFile = value; 
                CurRecord = null; 
                // devo verificare che dal security context siano eliminate le AC non più nello scope
                List<byte> toDel=new List<byte>();
                DF curDF=CurDF;
                foreach (var v in securityStatus.Keys) {
                    if (!IsInScope(curDF,securityStatus[v]))
                        toDel=new List<byte>();
                }
                foreach(var id in toDel)
                    securityStatus.Remove(id);

                if (curFile != null)
                    curFile.Owner.ObjectChanged(curFile, ChangeType.Selected);
            }
        }
        public Dictionary<byte, BSO> securityStatus;
        public CardContext() {
            securityStatus = new Dictionary<byte, BSO>();
        }

        bool IsInScope(DF scope,BSO obj) {
            if (scope == obj.Parent) return true;
            if (scope.Parent == null)
                return false;
            return IsInScope(scope.Parent, obj);
        }
    }

    public class CardHandler : ICardHandler 
    {
        public byte[] ATR { get { return card.ATR; } }
        IISO7816Card card;
        CardContext context;
        public CardContext Context { get { return context; } }
        protected Dictionary<ushort, ICardCommand> commandMap = new Dictionary<ushort, ICardCommand>();
        public CardHandler(ICard cardOwner)
        {
            this.card = cardOwner as IISO7816Card;
            context = new CardContext();
            context.CurFile = card.MasterFile;

            commandMap[0x0044] = new ActivateFile();            
            commandMap[0x00e2] = new AppendRecord();
            commandMap[0x9024] = new ChangeKeyData();
            commandMap[0x0024] = new ChangeReferenceData();
            commandMap[0x00e0] = new CreateFile();
            commandMap[0x0004] = new DeactivateFile();
            commandMap[0x0082] = new ExternalAuthenticate();
            commandMap[0x0046] = new GenerateKeyPair();
            commandMap[0x0084] = new GetChallenge();
            commandMap[0x8086] = new GiveRandom();
            commandMap[0x0022] = new ManageSecurityEnvironment();
            commandMap[0x002a] = new PerformSecurityOperation();
            commandMap[0x00da] = new PutData();
            commandMap[0x00b0] = new ReadBinary();
            commandMap[0x00b2] = new ReadRecord();
            commandMap[0x002c] = new ResetRetryCounter();
            commandMap[0x00a4] = new Select();
            commandMap[0x00d6] = new UpdateBinary();
            commandMap[0x00dc] = new UpdateRecord();
            commandMap[0x0020] = new Verify();
            

            foreach (var v in commandMap.Values) {
                v.Card = card;
                v.Handler = this;
            }

        }
        bool TestLogic(BSO bso, int ptr, out int exprLen)
        {
            byte[] logicExp = bso.Data;
            if (logicExp[ptr] == 0x00) { 
                int expLen1;
                int expLen2;
                bool a = TestLogic(bso, ptr - 1, out expLen1);
                bool b = TestLogic(bso, ptr - 1 - expLen1, out expLen2);
                exprLen = 1 + expLen1 + expLen2;
                return a && b;
            }
            if (logicExp[ptr] == 0xff)
            {
                int expLen1;
                int expLen2;
                bool a = TestLogic(bso, ptr - 1, out expLen1);
                bool b = TestLogic(bso, ptr - 1 - expLen1, out expLen2);
                exprLen = 1 + expLen1 + expLen2;
                return a || b;
            }
            exprLen = 1;
            byte condition=logicExp[ptr];
            if (context.securityStatus.ContainsKey(condition))
            {
                if (bso.Parent==context.CurDF)
                    return true;
                if (context.CurDF.GetChildBSO(condition,true) == bso.Parent.GetChildBSO(condition,true))
                    return true;
                return false;
            }
            return false;
        }

        public BSO getSMKey(IObjectWithSM obj, byte SMcondition) {
            if (obj==null)
                return null;
            byte SMkey=SM.NoSM;
            if (obj.SM!=null)
                SMkey = obj.SM[SMcondition];
            if (SMkey==SM.NoSM)
                return null;
            var df=obj is DF ? (obj as DF) : (obj as ICardObject).Parent;
            var key = df.GetChildBSO(Util.ToUShort(BSOClass.SM, SMkey), true);
            if (key == null)
                throw new ISO7816Exception(Error.ObjectNotFound);
            return key;
        }
        public bool IsVerifiedAC(ICardObject obj,byte ac) {
            if (!(obj is IObjectWithAC))
                throw new ISO7816Exception(Error.InternalError);
            IObjectWithAC acObj = obj as IObjectWithAC;
            byte condition = acObj.AC[ac];
            if (condition == AC.Never)
                return false;
            if (condition == AC.Always)
                return true;
            BSO refrencedObject=obj is DF ? (obj as DF).GetChildBSO(condition, true) : obj.Parent.GetChildBSO(condition, true);
            if (context.securityStatus.ContainsKey(condition))
            {
                BSO bso=context.securityStatus[condition];
                if (refrencedObject == bso)
                {
                    if (bso.CurValidityCounter == 0)
                    {
                        context.securityStatus.Remove((byte)bso.ID);
                        return false;
                    }
                    else if (bso.CurValidityCounter != 0xff)
                    {
                        bso.CurValidityCounter--;
                    }
                    return true;
                }
                return false;
            }
            else
            {
                // potrebbe essereun logical
                if (refrencedObject == null)
                    return false;
                if (refrencedObject.Algo == BSOAlgo.Logic)
                {
                    int exprLen;
                    return TestLogic(refrencedObject, refrencedObject.Data.Length - 1, out exprLen);
                }
                else
                    return false;
            }
        }
        bool VerifyKeyBSO(BSO bso, byte[] response, out bool verificationFailed)
        {
            verificationFailed = false;
            if (bso.Algo == BSOAlgo.PIN) {
                verificationFailed = !Util.CompareByteArray(response, bso.Data);
                return !verificationFailed;
            }
            else if (bso.Algo == BSOAlgo.MAC3_Test_SMSig) {
                if (context.Challenge == null)
                    throw new ISO7816Exception(Error.ConditionsOfUseNotSatisfied);
                var challenge = context.Challenge;
                context.Challenge = null;

                if ((challenge.Length & 0x7) != 0)
                    throw new ISO7816Exception(Error.ReferencedDataInvalidated);
                
                var cardResponse = getMAC(bso.Data, challenge);
                if (cardResponse.Length != response.Length)
                    throw new ISO7816Exception(Error.WrongLength);
                verificationFailed = !Util.CompareByteArray(response, cardResponse);
                return !verificationFailed;
            }
            else if (bso.Algo == BSOAlgo.RSA_DS_Test)
            {
                if (context.Challenge == null)
                    throw new ISO7816Exception(Error.ConditionsOfUseNotSatisfied);
                var challenge = context.Challenge;
                context.Challenge = null;

                if ((challenge.Length & 0x7) != 0)
                    throw new ISO7816Exception(Error.ReferencedDataInvalidated);

                BSO exp = bso.Parent.GetChildBSO((ushort)(bso.ID | 0x100));
                if (exp == null)
                    throw new ISO7816Exception(Error.ObjectNotFound);

                if (response.Length != (bso.Data.Length - 2))
                    throw new ISO7816Exception(Error.WrongLength);

                ByteArray dec = decryptRSA(new ByteArray(bso.Data).Sub(2), new ByteArray(exp.Data).Sub(2), response);
                if (dec[0] == 1)
                    dec = new ByteArray(0).Append(dec);
                try
                {
                    dec = ByteArray.RemoveBT1(dec);
                }
                catch {
                    verificationFailed = true;
                    return false;
                }
                verificationFailed = !Util.CompareByteArray(dec, challenge);
                return !verificationFailed;
            }
            throw new ISO7816Exception(Error.ObjectNotFound);
        }
        public bool VerifyBSO(BSO bso,byte[] response) 
        {
            bool isVerified = false;
            bool verificationFailed = false;
            try
            {
                if (bso.Blocked)
                    throw new ISO7816Exception(Error.BSOBlocked);
                isVerified = VerifyKeyBSO(bso, response, out verificationFailed);
                return isVerified;
            }
            finally {
                if (isVerified)
                {
                    bso.CurValidityCounter = (bso.ValidityCounter != 0xff && bso.ValidityCounter != 0) ? bso.ValidityCounter : 0xff;
                    context.securityStatus[bso.KeyID] = bso;

                    bso.CurErrorCounter = (bso.MaxErrorCounter != 0 && bso.MaxErrorCounter != 0x0f) ? bso.MaxErrorCounter : 0x0f;
                }
                else
                {
                    if (verificationFailed)
                    {
                        if (context.securityStatus.ContainsKey(bso.KeyID) &&
                            context.securityStatus[bso.KeyID] == bso)
                            context.securityStatus.Remove(bso.KeyID);

                        if (bso.CurErrorCounter != 0x0f)
                            bso.CurErrorCounter--;

                        if (bso.CurErrorCounter == 0x00)
                        {
                            bso.Blocked = true;
                        }
                    }
                }
            }
        }
        public void UnblockBSO(BSO bso)
        {
            bso.Blocked = false;
            bso.CurErrorCounter = (bso.MaxErrorCounter != 0 && bso.MaxErrorCounter != 0x0f) ? bso.MaxErrorCounter : 0x0f;
        }
        Random rand = new Random();
        public byte[] GetChallenge(byte len) 
        {
            context.Challenge=new byte[len];
            rand.NextBytes(context.Challenge);
            return context.Challenge;
        }
        public void GiveRandom(byte[] random)
        {
            if ((random.Length % 8) != 0)
                throw new ISO7816Exception(Error.DataFieldNotValid);
            context.Random = random;
        }
        public void RestoreSE(SecurityEnvironmenet se) 
        {
            context.CurrentSecurityEntironmentDF=context.CurDF;
            context.SEcomponents = new Dictionary<SecurityEnvironmentComponent, BSO>();
            byte comp_CDS = se.GetComponent(SecurityEnvironmentComponent.CDS);
            byte comp_TEST = se.GetComponent(SecurityEnvironmentComponent.TEST);
            byte comp_CON = se.GetComponent(SecurityEnvironmentComponent.CON);

            var bsoCDS = se.Parent.GetChildBSO(Util.ToUShort(BSOClass.PSO,comp_CDS), true);
            var bsoTEST = se.Parent.GetChildBSO(Util.ToUShort(BSOClass.Test, comp_TEST), true);
            var bsoCON = se.Parent.GetChildBSO(Util.ToUShort(BSOClass.PSO, comp_CON), true);

            if (bsoCDS != null) context.SEcomponents[SecurityEnvironmentComponent.CDS] = bsoCDS;
            if (bsoTEST != null) context.SEcomponents[SecurityEnvironmentComponent.TEST] = bsoTEST;
            if (bsoCON != null) context.SEcomponents[SecurityEnvironmentComponent.CON] = bsoCON;
        }
        public void RestoreSE(SecurityEnvironmentComponent comp,byte id)
        {
            if (context.CurrentSecurityEntironmentDF==null)
                throw new ISO7816Exception(Error.ConditionsOfUseNotSatisfied);
            byte keyClss=BSOClass.Unspecified;
            switch (comp) { 
                case SecurityEnvironmentComponent.CDS:
                    keyClss = BSOClass.PSO;
                    break;
                case SecurityEnvironmentComponent.CON:
                    keyClss = BSOClass.PSO;
                    break;
                case SecurityEnvironmentComponent.TEST:
                    keyClss = BSOClass.Test;
                    break;
                default:
                    throw new ISO7816Exception(Error.InternalError);
            }
            context.SEcomponents[comp] = context.CurDF.GetChildBSO(Util.ToUShort(keyClss,id));
        }
        public void GenerateKey(BSO privExpBso,BSO moduleBso, EFLinearTLV pubKeyEF, ushort pubExpLen)
        {
            BigInteger publicExponent;
            BigInteger privateExponent;
            BigInteger module;
            BigInteger.GenerateRSAKey(1024, pubExpLen, out publicExponent, out module, out privateExponent);

            ByteArray baPrivateExponent = new ByteArray(new byte[] { 0, 0 });
            baPrivateExponent=baPrivateExponent.Append(privateExponent.getBytes());
            baPrivateExponent[0]=(byte)(baPrivateExponent.Size-1);
            privExpBso.Data = baPrivateExponent;

            ByteArray baModule = new ByteArray(new byte[] { 0, 0 });
            baModule = baModule.Append(module.getBytes());
            baModule[0] = (byte)(baModule.Size - 1);
            moduleBso.Data = baModule;

            TLV modTlv = new TLV();
            modTlv.addTag(0x10, baModule);
            pubKeyEF.Append(modTlv.GetBytes());

            ByteArray baPublicExponent = new ByteArray(new byte[] { 0, 0 });
            baPublicExponent = baPublicExponent.Append(publicExponent.getBytes());
            baPublicExponent[0] = (byte)(baPublicExponent.Size - 1);

            TLV pubExpTlv = new TLV();
            pubExpTlv.addTag(0x11, baPublicExponent);
            pubKeyEF.Append(pubExpTlv.GetBytes());
        }
        public BSO GetEnvironmentKey(SecurityEnvironmentComponent component) 
        {
            if (context.CurrentSecurityEntironmentDF == null)
                return null;
            if (context.SEcomponents.ContainsKey(component))
                return context.SEcomponents[component];
            else
                return null;
        }
        public byte[] Decrypt(BSO key, byte[] data) 
        {
            if (key.Algo == BSOAlgo.DES3_Enc_SMEnc)
            {
                return ByteArray.RemoveISOPad(decrypt3DES(key.Data, data));
            }
            if (key.Algo == BSOAlgo.RSA_Enc)
            {
                BSO exp = key.Parent.GetChildBSO((ushort)(key.ID | 0x100));
                if (exp == null)
                    throw new ISO7816Exception(Error.ObjectNotFound);

                return decryptRSA(new ByteArray(key.Data).Sub(2), new ByteArray(exp.Data).Sub(2), data);
            }
            return Error.ObjectNotFound;
        }
        public byte[] Encrypt(BSO key, byte[] data)
        {
            if (key.Algo == BSOAlgo.DES3_Enc_SMEnc)
            {
                return encrypt3DES(key.Data, data);
            }
            if (key.Algo == BSOAlgo.RSA_Enc)
            {
                BSO exp = key.Parent.GetChildBSO((ushort)(key.ID | 0x100));
                if (exp == null)
                    throw new ISO7816Exception(Error.ObjectNotFound);

                return encryptRSA(new ByteArray(key.Data).Sub(2), new ByteArray(exp.Data).Sub(2), data);
            }
            return Error.ObjectNotFound;
        }
        public byte[] DigitalSignature(BSO key, byte[] data)
        {
            BSO key2 = key.Parent.GetChildBSO((ushort)(key.ID | 0x100));
            if (key2 == null)
                throw new ISO7816Exception(Error.ObjectNotFound);
            var module = new BigInteger(new ByteArray(key.Data).Sub(2));
            var privExp = new BigInteger(new ByteArray(key2.Data).Sub(2));
            ByteArray paddedData = ByteArray.BT1Pad(new ByteArray(data), key.Data.Length - 2);
            return new BigInteger(paddedData).modPow(privExp, module).getBytes();
        }

        public byte[] ProcessApdu(byte[] apdu)
        {
            try
            {
                Apdu Apdu = new Apdu(apdu);
                card.Log(Apdu);
                Apdu initialApdu = Apdu;
                ushort key = Util.ToUShort((byte)(Apdu.CLA & 0xf0),Apdu.INS);
                if (commandMap.ContainsKey(key))
                {
                    BSO sigIn, encIn, sigOut, encOut;
                    commandMap[key].getSMKeys(Apdu, out sigIn, out encIn, out sigOut, out encOut);
                    var challenge = context.Challenge;
                    if (sigIn != null || encIn != null || sigOut != null || encOut != null)
                    {
                        if (!Apdu.IsSM)
                            return Error.SMObjectMissing;
                        if (sigIn != null)
                            context.Challenge = null;
                        Apdu = Apdu.GetClearApdu(Apdu, encIn, sigIn, encOut, sigOut, challenge);
                    }
                    else
                    {
                        if (Apdu.IsSM)
                            return Error.ConditionsOfUseNotSatisfied;
                    }
                    var resp=commandMap[key].processCommand(Apdu);
                    card.Log(resp);
                    var random = context.Random;
                    if (encOut != null || sigOut != null || sigIn != null || encIn != null)
                    {
                        if (sigOut != null)
                            context.Challenge = null;
                        context.Random = null;
                        return Apdu.GetSMResponse(initialApdu, resp, encOut, sigOut, random);
                    }
                    else
                    {
                        return resp;
                    }
                }
                card.Log("Comando non riconosciuto");
                card.Log(Error.InsNotValid);
                return Error.InsNotValid;
            }
            catch (ISO7816Exception cex)
            {
                card.Log((byte[])cex.CardError);
                return cex.CardError;
            }
            catch (Exception ex)
            {
                card.Log(ex.ToString());
                return Error.InternalError;
            }
        }
        public byte[] ResetCard(bool warm)
        {
            context = new CardContext();
            context.CurFile = card.MasterFile;
            return card.ATR;
        }

        public ICardObject GetPath(DF startPath, byte[] path, int offset)
        {
            if (offset == path.Length - 2)
            {
                var term = startPath.GetChild(Util.ToUShort(path, path.Length - 2));
                return term;
            }
            var childDF = startPath.GetChildDF(Util.ToUShort(path, offset));
            if (childDF == null)
                return null;
            return GetPath(childDF, path, offset + 2);

        }

        public CardSelectable GetSelectablePath(DF startPath, byte[] path, int offset)
        {
            if (offset == path.Length - 2) {
                var term=startPath.GetChildEForDF(Util.ToUShort(path, path.Length - 2));
                return term;
            }
            var childDF=startPath.GetChildDF(Util.ToUShort(path,offset));
            if (childDF == null)
                return null;
            return GetSelectablePath(childDF, path, offset + 2);

        }

        public DF GetNamedDF(DF root, byte[] AID)
        {
            if (Util.CompareByteArray(root.AID, AID))
                return root;
            foreach (var v in root.Childs)
            {
                if (v is DF)
                {
                    DF namedDF = GetNamedDF(v as DF, AID);
                    if (namedDF != null)
                        return namedDF;
                }
            }
            return null;
        }

        static CardHandler() {
            td1.Padding = PaddingMode.Zeros;
            td2.Padding = PaddingMode.Zeros;
            td3.Padding = PaddingMode.Zeros;
        }

        static DESCryptoServiceProvider td1 = new DESCryptoServiceProvider();
        static DESCryptoServiceProvider td2 = new DESCryptoServiceProvider();
        static DESCryptoServiceProvider td3 = new DESCryptoServiceProvider();
        static TripleDESCryptoServiceProvider e1 = new TripleDESCryptoServiceProvider();
        static byte[] kMac1 = new byte[8];
        static byte[] kMac2 = new byte[8];
        static byte[] kMac3 = new byte[8];

        public static byte[] encryptRSA(byte[] module, byte[] exponent, byte[] data)
        {
            var moduleBi = new BigInteger(module);
            var privExpBi = new BigInteger(exponent);
            return new BigInteger(data).modPow(privExpBi, moduleBi).getBytes();
        }
        
        public static byte[] decryptRSA(byte[] module, byte[] exponent, byte[] data) {
            var moduleBi = new BigInteger(module);
            var privExpBi = new BigInteger(exponent);
            return new BigInteger(data).modPow(privExpBi, moduleBi).getBytes();
        }

        public static byte[] decrypt3DES(byte[] key, byte[] data)
        {
            e1.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            if (key.Length!=0x08 && 
                key.Length!=0x10 && 
                key.Length!=0x18)
                throw new Exception("Lunghezza della chiave errata");
            e1.Padding = PaddingMode.None;
            e1.Key = key;
            using (var enc = e1.CreateDecryptor())
            {
                MemoryStream ms = new MemoryStream();
                using (CryptoStream sc = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    sc.Write(data, 0, data.Length);
                    sc.Flush();
                }
                return ms.ToArray();
            }
        }
        
        public static byte[] encrypt3DES(byte[] key, byte[] data) {
            e1.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            if (key.Length!=0x08 && 
                key.Length!=0x10 && 
                key.Length!=0x18)
                throw new Exception("Lunghezza della chiave errata");
            e1.Padding = PaddingMode.None;
            e1.Key = key;
            using (var enc = e1.CreateEncryptor())
            {
                MemoryStream ms = new MemoryStream();
                int numWritten = 0;
                using (CryptoStream sc = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    sc.Write(data, 0, data.Length);
                    numWritten += data.Length;
                    sc.WriteByte(0x80);
                    numWritten ++;
                    while ((numWritten % 8) != 0) {
                        sc.WriteByte(0x0);
                        numWritten++;
                    }
                }
                return ms.ToArray();
            }
        }

        public static byte[] getMAC(byte[] key, byte[] data)
        {
            td1.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            td2.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            td3.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            if (key.Length == 24)
            {
                Array.Copy(key, 0, kMac1, 0, 8);
                Array.Copy(key, 8, kMac2, 0, 8);
                Array.Copy(key, 16, kMac3, 0, 8);
            }
            else if (key.Length == 16)
            {
                Array.Copy(key, 0, kMac1, 0, 8);
                Array.Copy(key, 8, kMac2, 0, 8);
                Array.Copy(key, 0, kMac3, 0, 8);
            }
            else if (key.Length == 8)
            {
                Array.Copy(key, 0, kMac1, 0, 8);
                Array.Copy(key, 0, kMac2, 0, 8);
                Array.Copy(key, 0, kMac3, 0, 8);
            }
            else throw new Exception("Lunghezza della chiave errata");
            td1.Key = kMac1;
            td2.Key = kMac2;
            td3.Key = kMac3;
            byte[] mid1;
            byte[] mid2;
            byte[] mid3;
            using(var enc1=td1.CreateEncryptor())
            {
                mid1 = enc1.TransformFinalBlock(data, 0, data.Length);
            }
            using (var enc2 = td2.CreateDecryptor())
            {
                mid2 = enc2.TransformFinalBlock(mid1, mid1.Length - 8, 8);
            }
            using (var enc3 = td3.CreateEncryptor())
            {
                mid3 = enc3.TransformFinalBlock(mid2, 0, 8);
            }
            return mid3;
        }

    }
}
