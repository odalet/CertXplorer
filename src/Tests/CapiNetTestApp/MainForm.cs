using System;
using System.Windows.Forms;
using Delta.CapiNet.Logging;
using CapiNetTestApp.Tests;
using System.Diagnostics.CodeAnalysis;

namespace CapiNetTestApp
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Windows Forms conventions")]
    public partial class MainForm : Form
    {
        public class LogService : CapiNetLogger.ILogService
        {
            private readonly MainForm owner;

            public LogService(Type type, MainForm form)
            {
                Type = type;
                owner = form;
            }

            public Type Type { get; }

            public void Log(string level, string message, Exception exception)
            {
                if (exception == null)
                    owner.Log(string.Format("{0} - {1}", level, message));
                else owner.LogException(exception);
            }
        }

        public MainForm() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Initialize CapiNet logging
            CapiNetLogger.LogServiceBuilder = t => new LogService(t, this);
        }

        internal void LogException(Exception exception) => logbox.LogException(exception);
        internal void Log(string message) => logbox.LogMessage(message);

        private void certificateAndCrlTestButton_Click(object sender, EventArgs e) => CertificateAndCrlTest.Test();
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
    }
}
