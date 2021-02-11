using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Delta.SmartCard.Logging;

using PCSC;
using PCSC.Iso7816;

namespace Delta.SmartCard
{
    public class PcscSmartCardReader : IDisposable
    {
        private static ILogService log = LogManager.GetLogger<PcscSmartCardReader>();

        // What does this change???
        private const SCardReaderDisposition disconnectAction = SCardReaderDisposition.Leave;

        private readonly string name;
        private readonly SCardScope scope;
        private readonly SCardShareMode shareMode = SCardShareMode.Shared;
        private readonly SCardProtocol protocol = SCardProtocol.Any;

        private SCardContext context = null;
        private IsoReader reader = null;

        internal PcscSmartCardReader(string readerName, SCardScope scardScope = SCardScope.System)
        {
            if (string.IsNullOrEmpty("readerName"))
                throw new ArgumentNullException("readerName");

            name = readerName;
            scope = scardScope;

            CreateContextAndReader();
        }

        private byte[] SendCommand(CommandApdu command)
        {
            var response = reader.Transmit(command);
            if (!response.HasData)
                return new byte[] { response.SW1, response.SW2 };

            var data = response.GetData();
            var result = new byte[data.Length + 2];
            Buffer.BlockCopy(data, 0, result, 0, data.Length);
            result[result.Length - 2] = response.SW1;
            result[result.Length - 1] = response.SW2;

            return result;
        }

        #region Other public methods (to include in the interface!)

        public Dictionary<string, byte[]> GetInformation()
        {
            if (!IsCardOpened)
                return null;

            var dictionary = new Dictionary<string, byte[]>();
            byte[] result;
            foreach (var enumValue in Enum.GetValues(typeof(SCardAttribute)).Cast<SCardAttribute>())
            {
                try
                {
                    reader.Reader.GetAttrib(enumValue, out result);
                    if (result != null)
                        dictionary.Add(enumValue.ToString(), (byte[])result.Clone());
                }
                catch (Exception ex)
                {
                    var debugEx = ex;
                }
                finally { result = null; }
            }

            // Add Status information

            try
            {
                string[] names;
                SCardState state;
                SCardProtocol protocol;
                byte[] atr;

                reader.Reader.Status(out names, out state, out protocol, out atr);

                if (names != null && names.Length > 0) dictionary.Add("Name", Encoding.Default.GetBytes(
                    string.Join(", ", names.Where(n => !string.IsNullOrEmpty(n)).ToArray())));
                dictionary.Add("State", Encoding.Default.GetBytes(state.ToString()));
                dictionary.Add("Protocol", Encoding.Default.GetBytes(protocol.ToString()));
                dictionary.Add("Atr", atr);
            }
            catch (Exception ex)
            {
                var debugEx = ex;
            }

            return dictionary;
        }

        public byte[] SendCommand(byte cla, byte ins, byte p1, byte p2) // Case 1
        {
            return SendCommand(new CommandApdu(IsoCase.Case1, reader.ActiveProtocol)
            {
                CLA = cla,
                INS = ins,
                P1 = p1,
                P2 = p2
            });
        }

        public byte[] SendCommand(byte cla, byte ins, byte p1, byte p2, int le) // Case 2
        {
            var isT0 = reader.ActiveProtocol == SCardProtocol.T0;

            if (isT0 && le > byte.MaxValue) throw new ArgumentException(string.Format(
                "Expected Length (Le) is too long (protocol is T=0). Maximum value is {0}", byte.MaxValue), "le");

            if (le > ushort.MaxValue) throw new ArgumentException(string.Format(
                "Expected Length (Le) is too long. Maximum value is {0}", ushort.MaxValue), "le");

            var isoCase = le < byte.MaxValue ? IsoCase.Case2Short : IsoCase.Case2Extended;
            return SendCommand(new CommandApdu(isoCase, reader.ActiveProtocol)
            {
                CLA = cla,
                INS = ins,
                P1 = p1,
                P2 = p2,
                Le = le
            });
        }

        public byte[] SendCommand(byte cla, byte ins, byte p1, byte p2, byte[] data) // Case 3
        {
            if (data == null || data.Length == 0)
                return SendCommand(cla, ins, p1, p2); //  Case 1

            var isT0 = reader.ActiveProtocol == SCardProtocol.T0;

            if (isT0 && data.Length > byte.MaxValue) throw new ArgumentException(string.Format(
                "Data is too long (protocol is T=0). Maximum size is {0}", byte.MaxValue), "data");

            if (data.Length > ushort.MaxValue) throw new ArgumentException(string.Format(
                "Data is too long. Maximum size is {0}", ushort.MaxValue), "data");

            var isoCase = data.Length < byte.MaxValue ? IsoCase.Case3Short : IsoCase.Case3Extended;
            return SendCommand(new CommandApdu(isoCase, reader.ActiveProtocol)
            {
                CLA = cla,
                INS = ins,
                P1 = p1,
                P2 = p2,
                Data = data
            });
        }

        public byte[] SendCommand(byte cla, byte ins, byte p1, byte p2, byte[] data, int le)
        {
            if (data == null || data.Length == 0)
                return SendCommand(cla, ins, p1, p2, le); //  Case 2

            var isT0 = reader.ActiveProtocol == SCardProtocol.T0;

            if (isT0 && data.Length > byte.MaxValue) throw new ArgumentException(string.Format(
                "Data is too long (protocol is T=0). Maximum size is {0}", byte.MaxValue), "data");

            if (data.Length > ushort.MaxValue) throw new ArgumentException(string.Format(
                "Data is too long. Maximum size is {0}", ushort.MaxValue), "data");

            if (isT0 && le > byte.MaxValue) throw new ArgumentException(string.Format(
                "Expected Length (Le) is too long (protocol is T=0). Maximum value is {0}", byte.MaxValue), "le");

            if (le > ushort.MaxValue) throw new ArgumentException(string.Format(
                "Expected Length (Le) is too long. Maximum value is {0}", ushort.MaxValue), "le");

            var isoCase = data.Length < byte.MaxValue ? IsoCase.Case4Short : IsoCase.Case4Extended;
            return SendCommand(new CommandApdu(isoCase, reader.ActiveProtocol)
            {
                CLA = cla,
                INS = ins,
                P1 = p1,
                P2 = p2,
                Data = data,
                Le = le
            });
        }

        #endregion

        public void OpenCard()
        {
            EnsureInitialized();

            if (reader.Reader.IsConnected)
                return;

            TryPcsc(() =>
                reader.Connect(name, shareMode, protocol));
        }

        public void CloseCard()
        {
            if (!reader.Reader.IsConnected)
                return;

            reader.Disconnect(disconnectAction);
        }

        public bool IsCardOpened
        {
            get { return reader != null && reader.Reader.IsConnected; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (reader != null)
            {
                if (IsCardOpened)
                {
                    try
                    {
                        CloseCard();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                try
                {
                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                finally { reader = null; }
            }

            if (context != null)
            {
                try
                {
                    context.Dispose();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                finally { context = null; }
            }
        }

        #endregion

        private void CreateContextAndReader()
        {
            context = new SCardContext();
            context.Establish(scope);
            context.EnsureOK();

            reader = new IsoReader(context);
        }

        private void EnsureInitialized()
        {
            if (reader != null)
                return;

            throw new InvalidOperationException("This instance is not initialized. Make sure you don't reuse a disposed instance.");
        }

        private bool TryPcsc(Action action, bool throwException = true)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                if (throwException)
                    throw;

                log.Error(ex);
                return false;
            }
        }
    }
}
