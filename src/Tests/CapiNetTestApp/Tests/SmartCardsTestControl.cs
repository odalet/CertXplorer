using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Delta.SmartCard;
using PCSC.Iso7816;

namespace CapiNetTestApp.Tests
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms conventions")]
    public partial class SmartCardsTestControl : UserControl
    {
        private sealed class Apdu
        {
            public Apdu(byte cla, InstructionCode ins, byte p1, byte p2, byte? le = null) : this(cla, (byte)ins, p1, p2, le) { }
            private Apdu(byte cla, byte ins, byte p1, byte p2, byte? le = null)
            {
                Cla = cla;
                Ins = ins;
                P1 = p1;
                P2 = p2;
                Le = le;
            }

            public byte Cla { get; }
            public byte Ins { get; }
            public byte P1 { get; }
            public byte P2 { get; }
            public byte? Le { get; }

            public byte[] Execute(PcscSmartCardReader reader)
            {
                try
                {
                    return Le.HasValue ? 
                        reader.SendCommand(Cla, Ins, P1, P2, Le.Value) : 
                        reader.SendCommand(Cla, Ins, P1, P2);
                }
                catch (Exception ex)
                {
                    Program.LogException(ex);
                    return new byte[0];
                }
            }

            public override string ToString()
            {
                var result = $"CLA={Cla:X2} INS={Ins:X2} P1={P1:X2} P2={P2:X2}";
                if (Le.HasValue)
                    result += $" Le={Le.Value:X2}";
                return result;
            }
        }

        private sealed class SCResult
        {
            private readonly byte[] data;

            public SCResult(byte[] receivedData)
            {
                if (receivedData == null || receivedData.Length < 2)
                    throw new ArgumentException("Invalid Data");
                data = receivedData;
            }

            public byte SW1 => data[data.Length - 2];
            public byte SW2 => data[data.Length - 1];

            public override string ToString()
            {
                var builder = new StringBuilder();

                var sw1Meaning = "?";
                if (Enum.GetValues(typeof(SW1Code)).Cast<byte>().Contains(SW1))
                    sw1Meaning = ((SW1Code)SW1).ToString();

                _ = builder.Append($"SW1={SW1:X2} [{sw1Meaning}], SW2={SW2:X2} [Remaining Bytes]");
                if (data.Length > 2)
                {
                    for (var i = 0; i < data.Length - 2; i++)
                    {
                        if (i % 16 == 0)
                            builder.AppendLine();
                        builder.Append($"{data[i]:X2} ");
                    }
                }

                builder.AppendLine();
                return builder.ToString();
            }
        }

        public SmartCardsTestControl()
        {
            InitializeComponent();

            testsPanel.Enabled = false;
            smartCardReaderChooser.SmartCardReaderConnected += (s, _) => OnConnected();
            smartCardReaderChooser.SmartCardReaderDisconnected += (s, _) => OnDisconnected();
        }

        private PcscSmartCardReader Reader => smartCardReaderChooser.SmartCardReader;

        private void OnConnected() => testsPanel.Enabled = true;
        private void OnDisconnected() => testsPanel.Enabled = false;

        private void getChallengeButton_Click(object sender, EventArgs e)
        {
            var apdu = new Apdu(0, InstructionCode.GetChallenge, 0, 0, 8);
            Program.Log(string.Format("Send APDU with \"GET CHALLENGE\" command:\r\n\t{0}", apdu));

            var result = apdu.Execute(Reader);
            if (result == null)
                Program.Log("No result.");
            else Program.Log(string.Format("Result: {0}", new SCResult(result)));
        }

        private void getReaderInformationButton_Click(object sender, EventArgs e)
        {
            Program.Log("Retrieve Reader information:");
            var result = Reader.GetInformation();
            if (result == null || result.Count == 0)
            {
                Program.Log("No Data!");
                return;
            }

            foreach (var k in result.Keys)
            {
                var v = result[k];
                if (v == null || v.Length == 0)
                    Program.Log(string.Format("{0}: NULL", k));
                else
                {
                    var text = Encoding.Default.GetString(v).Replace('\0', '.');
                    var bytes = string.Join(" ", v.Select(b => b.ToString("X2")));

                    Program.Log(string.Format("{0}: {1} / {2}", k, text, bytes));
                }
            }
        }
    }
}
