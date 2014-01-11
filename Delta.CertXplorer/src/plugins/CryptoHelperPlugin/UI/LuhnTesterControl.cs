using System;
using System.Windows.Forms;

using Delta.CapiNet.Cryptography;

namespace CryptoHelperPlugin.UI
{
    public partial class LuhnTesterControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LuhnTesterControl"/> class.
        /// </summary>
        public LuhnTesterControl()
        {
            InitializeComponent();
        }

        private void inputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var c = e.KeyChar;
            if ((c < '0' || c > '9') && c != '\b')
                e.Handled = true;
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            var number = inputBox.Text;
            if (string.IsNullOrEmpty(number)) return;

            LogLn(string.Format("Validating number {0}:", number));
            try
            {
                var ok = Luhn.Validate(number);
                if (ok) LogLn("\tOK");
                else LogLn(string.Format(
                    "\tKO: correct last number should be {0}", Luhn.Compute(number.Substring(0, number.Length - 1))));
            }
            catch (Exception ex)
            {
                Log(string.Format("\tError: {0}", ex.Message));
            }

            LogLn();
        }

        private void computeButton_Click(object sender, EventArgs e)
        {
            var number = inputBox.Text;
            if (string.IsNullOrEmpty(number)) return;

            LogLn(string.Format("Computing Luhn checksum for number {0}:", number));
            try
            {
                var result = Luhn.Compute(number);
                LogLn(string.Format("\tResult: {0} - Complete number is {1}", result, number + result.ToString()));
            }
            catch (Exception ex)
            {
                Log(string.Format("\tError: {0}", ex.Message));
            }

            LogLn();
        }

        private void clearButton_Click(object sender, EventArgs e) { logbox.Clear(); }

        private void LogLn(string text = "") { Log(text + "\r\n"); }

        /// <summary>
        /// Logs the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void Log(string text)
        {
            logbox.AppendText(text);
            logbox.Select(logbox.Text.Length, 0);
        }
    }
}
