using System.IO;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
namespace VirtualSmartCard
{
    public interface ICardPlugin
    {
        IEnumerable<ICardImplementation> Implementations { get; }
    }

    public interface ICardImplementation {
        ICard GetCardObject();
    }

    public interface ICardHandler
    {
        byte[] ProcessApdu(byte[] apdu);
        byte[] ResetCard(bool warm);
        byte[] ATR { get; }
    }
    public interface ICard
    {
        ICardImplementation Implementation { get; }
        event Action<Object> log;
        void Log(object logMsg);
        void Log(byte[] logMsg);

        Control GetUI();
        ICardHandler Handler { get; }
        string Name { get; set; }

        void NewCard();
        byte[] ATR { get; }
    }

}