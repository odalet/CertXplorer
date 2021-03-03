using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Delta.SmartCard;

namespace CapiNetTestApp.UI
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Windows Forms conventions")]
    public partial class SmartCardReaderChooser : UserControl
    {
        // See https://stackoverflow.com/questions/34664/designmode-with-nested-controls
        public bool IsInDesignMode
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;

                // IsDesignerHosted part
                Control current = this;
                while (current != null)
                {
                    if (current.Site != null && current.Site.DesignMode)
                        return true;
                    current = current.Parent;
                }

                return false;
            }
        }

        public SmartCardReaderChooser()
        {
            InitializeComponent();
            if (IsInDesignMode) return;

            Application.ApplicationExit += (s, _) =>
            {
                void @try(Action a)
                {
                    try { a(); }
                    catch { /* Nothing to do here */ }
                }

                if (SmartCardReader != null)
                {
                    @try(() => SmartCardReader.CloseCard());
                    @try(() => SmartCardReader.Dispose());
                    SmartCardReader = null;
                }
            };

            readersBox.SelectedValueChanged += (s, _) => RefreshUI();
            refreshReadersButton.Click += (s, _) => FillReadersList();
            openCardButton.Click += (s, _) => OpenCard();
            closeCardButton.Click += (s, _) => CloseCard();
        }

        public event EventHandler SmartCardReaderConnected;
        public event EventHandler SmartCardReaderDisconnected;

        public PcscSmartCardReader SmartCardReader { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsInDesignMode) return;

            try
            {
                FillReadersList();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(this, $"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            RefreshUI();
        }

        protected virtual void OnSmartCardReaderConnected() => SmartCardReaderConnected?.Invoke(this, EventArgs.Empty);
        protected virtual void OnSmartCardReaderDisconnected() => SmartCardReaderDisconnected?.Invoke(this, EventArgs.Empty);

        private void OpenCard()
        {
            var name = readersBox.SelectedItem as string;
            if (string.IsNullOrEmpty(name))
                return;

            if (SmartCardReader != null)
            {
                SmartCardReader.Dispose();
                SmartCardReader = null;
            }

            try
            {
                SmartCardReader = PcscSmartCardReaderFactory.CreateDevice(name);
                SmartCardReader.OpenCard();
                Program.Log(string.Format("Card Opened: {0}", SmartCardReader.IsCardOpened));
                OnSmartCardReaderConnected();
            }
            catch (Exception ex)
            {
                Program.LogException(ex);
                if (SmartCardReader != null)
                    SmartCardReader.Dispose();
                SmartCardReader = null;
            }

            RefreshUI();
        }

        private void CloseCard()
        {
            try
            {
                SmartCardReader.CloseCard();
                Program.Log(string.Format("Card Opened: {0}", SmartCardReader.IsCardOpened));
            }
            catch (Exception ex)
            {
                Program.LogException(ex);
            }
            finally
            {
                if (SmartCardReader != null)
                    SmartCardReader.Dispose();
                SmartCardReader = null;
                OnSmartCardReaderDisconnected();
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            readersBox.Enabled = refreshReadersButton.Enabled = SmartCardReader == null;

            openCardButton.Enabled = SmartCardReader == null && readersBox.SelectedItem != null;
            closeCardButton.Enabled = SmartCardReader != null;
        }

        private void FillReadersList()
        {
            readersBox.Items.Clear();
            readersBox.Items.AddRange(
                PcscSmartCardReaderFactory.EnumerateDevices());
        }
    }
}
