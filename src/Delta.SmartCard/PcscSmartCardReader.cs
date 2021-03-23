using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delta.SmartCard.Logging;
using PCSC;
using PCSC.Iso7816;

namespace Delta.SmartCard
{
    public sealed class PcscSmartCardReader : IDisposable
    {
        private static readonly ILogService log = LogManager.GetLogger<PcscSmartCardReader>();

        // What does this change???
        private const SCardReaderDisposition disconnectAction = SCardReaderDisposition.Leave;

        private readonly string name;
        private readonly SCardScope scope;
        private readonly SCardShareMode shareMode = SCardShareMode.Shared;
        private readonly SCardProtocol protocol = SCardProtocol.Any;

        private SCardContext context = null;
        private IsoReader reader = null;
        private SCardReader scReader = null;

        internal PcscSmartCardReader(string readerName, SCardScope scardScope = SCardScope.System)
        {
            if (string.IsNullOrEmpty(readerName))
                throw new ArgumentNullException(nameof(readerName));

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

        public Dictionary<string, byte[]> GetInformation()
        {
            var dictionary = new Dictionary<string, byte[]>();
            if (!IsCardOpened)
                return dictionary;

            foreach (var enumValue in Enum.GetValues(typeof(SCardAttribute)).Cast<SCardAttribute>())
            {
                try
                {
                    _ = scReader.GetAttrib(enumValue, out var result);
                    if (result != null)
                        dictionary.Add(enumValue.ToString(), (byte[])result.Clone());
                }
                catch (Exception ex)
                {
                    log.Debug($"Error: {ex.Message}", ex);
                }
            }

            // Add Status information
            try
            {

                _ = scReader.Status(out var names, out var state, out var scardProtocol, out var atr);

                if (names != null && names.Length > 0) dictionary.Add("Name", Encoding.Default.GetBytes(
                    string.Join(", ", names.Where(n => !string.IsNullOrEmpty(n)).ToArray())));

                dictionary.Add("State", Encoding.Default.GetBytes(state.ToString()));
                dictionary.Add("Protocol", Encoding.Default.GetBytes(scardProtocol.ToString()));
                dictionary.Add("Atr", atr);
            }
            catch (Exception ex)
            {
                log.Debug($"Error: {ex.Message}", ex);
            }

            return dictionary;
        }

        // Case 1
        public byte[] SendCommand(byte cla, byte ins, byte p1, byte p2) => SendCommand(new CommandApdu(IsoCase.Case1, reader.ActiveProtocol)
        {
            CLA = cla,
            INS = ins,
            P1 = p1,
            P2 = p2
        });

        // Case 2
        public byte[] SendCommand(byte cla, byte ins, byte p1, byte p2, int le) 
        {
            var isT0 = reader.ActiveProtocol == SCardProtocol.T0;

            if (isT0 && le > byte.MaxValue) throw new ArgumentException(
                $"Expected Length (Le) is too long (protocol is T=0). Maximum value is {byte.MaxValue}", nameof(le));

            if (le > ushort.MaxValue) throw new ArgumentException(
                $"Expected Length (Le) is too long. Maximum value is {ushort.MaxValue}", nameof(le));

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

        // Case 3
        public byte[] SendCommand(byte cla, byte ins, byte p1, byte p2, byte[] data) 
        {
            if (data == null || data.Length == 0) return SendCommand(cla, ins, p1, p2); //  Case 1

            var isT0 = reader.ActiveProtocol == SCardProtocol.T0;

            if (isT0 && data.Length > byte.MaxValue) throw new ArgumentException(
                $"Data is too long (protocol is T=0). Maximum size is {byte.MaxValue}", nameof(data));

            if (data.Length > ushort.MaxValue) throw new ArgumentException(
                $"Data is too long. Maximum size is {ushort.MaxValue}", nameof(data));

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
            if (data == null || data.Length == 0) return SendCommand(cla, ins, p1, p2, le); //  Case 2

            var isT0 = reader.ActiveProtocol == SCardProtocol.T0;

            if (isT0 && data.Length > byte.MaxValue) throw new ArgumentException(
                $"Data is too long (protocol is T=0). Maximum size is {byte.MaxValue}", nameof(data));

            if (data.Length > ushort.MaxValue) throw new ArgumentException(
                $"Data is too long. Maximum size is {ushort.MaxValue}", nameof(data));

            if (isT0 && le > byte.MaxValue) throw new ArgumentException(
                $"Expected Length (Le) is too long (protocol is T=0). Maximum value is {byte.MaxValue}", nameof(le));

            if (le > ushort.MaxValue) throw new ArgumentException(
                $"Expected Length (Le) is too long. Maximum value is {ushort.MaxValue}", nameof(le));

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

        public void OpenCard()
        {
            EnsureInitialized();

            if (IsCardOpened)
                return;

            _ = TryPcsc(() => reader.Connect(name, shareMode, protocol));
        }

        public void CloseCard()
        {
            if (!IsCardOpened)
                return;

            _ = scReader.Disconnect(disconnectAction);
        }

        public bool IsCardOpened => scReader != null && scReader.IsConnected;

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
                    scReader.Dispose();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                finally { scReader = null; }

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

        private void CreateContextAndReader()
        {
            context = new SCardContext();
            context.Establish(scope);
            _ = context.EnsureOK();

            reader = new IsoReader(context);
            scReader = new SCardReader(context);
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
