using System;
using System.Windows.Forms;
using Delta.CapiNet;
using Delta.CapiNet.Logging;
using TestCapiNet.Tests;

namespace TestCapiNet
{
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

            #region ILogService Members

            public Type Type { get; private set; }

            public void Log(string level, string message, Exception exception)
            {
                if (exception == null)
                    owner.Log(string.Format("{0} - {1}", level, message));
                else owner.LogException(exception);
            }

            #endregion
        }

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Initialize CapiNet logging
            CapiNetLogger.LogServiceBuilder = t => new LogService(t, this);
        }

        internal void LogException(Exception exception)
        {
            logbox.LogException(exception);
        }

        internal void Log(string message)
        {
            logbox.LogMessage(message);
        }

        private void certificateAndCrlTestButton_Click(object sender, EventArgs e)
        {
            CertificateAndCrlTest.Test();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
