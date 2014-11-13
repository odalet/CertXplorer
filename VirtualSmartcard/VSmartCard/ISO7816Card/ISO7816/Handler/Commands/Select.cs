using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISO7816.FileSystem;

namespace ISO7816.Handler.Commands
{
    public class Select : ICardCommand
    {
        protected IISO7816Card card;
        public IISO7816Card Card { set { card = value; } }
        protected CardHandler handler;
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
            if (apdu.P2 != 0)
                return Error.P1OrP2NotValid;
            CardSelectable obj = null;
            switch (apdu.P1)
            {
                case 0:
                    if (apdu.Data == null || apdu.Data.Length == 0)
                        obj = card.MasterFile;
                    else
                    {
                        if (apdu.Data.Length != 2)
                            return Error.DataFieldNotValid;
                        ushort id = Util.ToUShort(apdu.Data);
                        if (id == 0x3f00)
                            obj = card.MasterFile;
                        else
                        {
                            obj = context.CurDF.GetChildEForDF(id);
                            if (obj == null && context.CurDF.Parent != null)
                                obj = context.CurDF.Parent.GetChildEForDF(id);
                            if (obj == null && context.CurDF.Parent != null && id == context.CurDF.ID)
                                obj = context.CurDF.Parent;
                        }
                    }
                    if (obj == null)
                        return Error.FileNotFound;

                    context.CurFile = obj;
                    break;
                case 1:
                    if (apdu.Data == null || apdu.Data.Length != 2)
                        return Error.DataFieldNotValid;
                    ushort id2 = Util.ToUShort(apdu.Data);
                    obj = context.CurDF.GetChildDF(id2);
                    if (obj == null)
                        return Error.FileNotFound;
                    context.CurFile = obj;
                    break;
                case 2:
                    if (apdu.Data == null || apdu.Data.Length != 2)
                        return Error.DataFieldNotValid;
                    ushort id3 = Util.ToUShort(apdu.Data);
                    obj = context.CurDF.GetChildDF(id3);
                    if (obj == null)
                        return Error.FileNotFound;
                    context.CurFile = obj;
                    break;
                case 3:
                    if (apdu.Data != null && apdu.Data.Length != 0)
                        return Error.DataFieldNotValid;

                    if (context.CurDF.Parent == null)
                        return Error.FileNotFound;

                    context.CurFile = context.CurDF.Parent;
                    break;
                case 4:
                    if (apdu.Data == null || apdu.Data.Length == 0)
                        return Error.DataFieldNotValid;

                    DF namedDF = handler.GetNamedDF(card.MasterFile, apdu.Data);
                    if (namedDF == null)
                        return Error.FileNotFound;

                    context.CurFile = namedDF;
                    break;
                case 8:
                    if (apdu.Data == null || apdu.Data.Length == 0)
                        obj = card.MasterFile;
                    else
                        if (apdu.Data != null && apdu.Data.Length == 2 && Util.ToUShort(apdu.Data) == 0x3f00)
                            obj = card.MasterFile;
                        else
                        {
                            if ((apdu.Data.Length % 2) != 0)
                                return Error.DataFieldNotValid;
                            obj = handler.GetSelectablePath(card.MasterFile, apdu.Data, 0);
                            if (obj == null)
                                return Error.FileNotFound;
                        }
                    context.CurFile = obj;
                    break;
                case 9:
                    if (apdu.Data == null || apdu.Data.Length == 0)
                        return Error.DataFieldNotValid;
                    else
                    {
                        if ((apdu.Data.Length % 2) != 0)
                            return Error.DataFieldNotValid;
                        if (context.CurDF == null)
                            return Error.ClaNotValid;
                        obj = handler.GetSelectablePath(context.CurDF, apdu.Data, 0);
                        if (obj == null)
                            return Error.FileNotFound;
                    }
                    context.CurFile = obj;
                    break;
                default:
                    return Error.P1OrP2NotValid;
            }
            if (context.CurFile == null)
                return Error.FileNotFound;
            var outData = (context.CurFile as IObjectWithFCI).FCI;
            TLV tlv = new TLV();
            TLV tlv2 = new TLV();
            tlv2.elems.AddRange(outData);
            tlv.addTag(0x6f, tlv2.GetBytes());
            return Util.Response(tlv.GetBytes(), Error.Ok);
        }
    }
}
