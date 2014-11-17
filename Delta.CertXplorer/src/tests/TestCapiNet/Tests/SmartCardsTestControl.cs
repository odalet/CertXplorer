using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using PCSC.Iso7816;
using Delta.SmartCard;

namespace TestCapiNet.Tests
{
    public partial class SmartCardsTestControl : UserControl
    {
        private class Apdu
        {
            public Apdu(byte cla, InstructionCode ins, byte p1, byte p2, byte? le = null) : this(cla, (byte)ins, p1, p2, le) { }

            public Apdu(byte cla, byte ins, byte p1, byte p2, byte? le = null)
            {
                Cla = cla;
                Ins = ins;
                P1 = p1;
                P2 = p2;
                Le = le;
            }

            public byte Cla { get; private set; }

            public byte Ins { get; private set; }

            public byte P1 { get; private set; }

            public byte P2 { get; private set; }

            public byte? Le { get; private set; }

            public byte[] Data { get; set; }

            public byte[] Execute(PcscSmartCardReader reader)
            {
                try
                {
                    if (Le.HasValue)
                    {
                        if (Data != null && Data.Length > 0)
                            return reader.SendCommand(Cla, Ins, P1, P2, Data, Le.Value);
                        else return reader.SendCommand(Cla, Ins, P1, P2, Le.Value);
                    }
                    else
                    {
                        if (Data != null && Data.Length > 0)
                            return reader.SendCommand(Cla, Ins, P1, P2, Data);
                        else return reader.SendCommand(Cla, Ins, P1, P2);
                    }
                }
                catch (Exception ex)
                {
                    Program.LogException(ex);
                    return null;
                }
            }

            public override string ToString()
            {
                var result = string.Format("CLA={0:X2} INS={1:X2} P1={2:X2} P2={3:X2}", Cla, Ins, P1, P2);

                if (Data != null && Data.Length > 0)
                {
                    result += string.Format(" Lc={0:X2} <", Data.Length);
                    const int max = 8;

                    for (int i = 0; i < max; i++)
                    {
                        if (i >= Data.Length)
                            break;
                        result += string.Format(" {0:X2}", Data[i]);
                    }

                    result += Data.Length > max ? "... >" : " >";
                }

                if (Le.HasValue)
                    result += string.Format(" Le={0:X2}", Le.Value);

                return result;
            }
        }

        private class SCResult
        {
            private byte[] data = null;

            public SCResult(byte[] receivedData)
            {
                if (receivedData == null || receivedData.Length < 2)
                    throw new ArgumentException("Invalid Data");
                data = receivedData;
            }

            public byte SW1
            {
                get { return data[data.Length - 2]; }
            }

            public byte SW2
            {
                get { return data[data.Length - 1]; }
            }

            public override string ToString()
            {
                var builder = new StringBuilder();

                var sw1Meaning = "?";
                if (Enum.GetValues(typeof(SW1Code)).Cast<byte>().Contains(SW1))
                    sw1Meaning = ((SW1Code)SW1).ToString();

                builder.AppendFormat("SW1={0:X2} [{1}], SW2={2:X2} [Remaining Bytes]", SW1, sw1Meaning, SW2);
                if (data.Length > 2)
                {
                    for (int i = 0; i < data.Length - 2; i++)
                    {
                        if (i % 16 == 0)
                            builder.AppendLine();
                        builder.AppendFormat("{0:X2} ", data[i]);
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

        private PcscSmartCardReader Reader
        {
            get { return smartCardReaderChooser.SmartCardReader; }
        }

        private void OnConnected()
        {
            testsPanel.Enabled = true;
        }

        private void OnDisconnected()
        {
            testsPanel.Enabled = false;
        }

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
