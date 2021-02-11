using System.Collections.Generic;
using PCSC;

namespace Delta.SmartCard
{
    public static class PcscSmartCardReaderFactory
    {
        public static string[] EnumerateDevices()
        {
            using (var context = new SCardContext())
            {
                context.Establish(SCardScope.System);
                context.EnsureOK();

                return context.GetReaders();
            }            
        }

        public static PcscSmartCardReader CreateDevice(string readerName)
        {
            return new PcscSmartCardReader(readerName);
        }
    }
}
