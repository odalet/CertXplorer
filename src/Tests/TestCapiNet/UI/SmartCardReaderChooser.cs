using System;
using System.ComponentModel;
using System.Windows.Forms;
using Delta.SmartCard;

namespace TestCapiNet.UI
{
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

        //private string curentReaderName = null;
        private PcscSmartCardReader reader = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartCardReaderChooser"/> class.
        /// </summary>
        public SmartCardReaderChooser()
        {
            InitializeComponent();
            if (IsInDesignMode) return;

            Application.ApplicationExit += (s, _) =>
            {
                Action<Action> t = a =>
                {
                    try { a(); }
                    catch { }
                };

                if (reader != null)
                {
                    t(() => reader.CloseCard());
                    t(() => reader.Dispose());
                    reader = null;
                }
            };

            readersBox.SelectedValueChanged += (s, _) => RefreshUI();
            refreshReadersButton.Click += (s, _) => FillReadersList();
            openCardButton.Click += (s, _) => OpenCard();
            closeCardButton.Click += (s, _) => CloseCard();
        }

        public event EventHandler SmartCardReaderConnected;
        public event EventHandler SmartCardReaderDisconnected;

        public PcscSmartCardReader SmartCardReader => reader;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsInDesignMode) return;

            FillReadersList();
            RefreshUI();
        }
        
        protected virtual void OnSmartCardReaderConnected()
        {
            if (SmartCardReaderConnected != null)
                SmartCardReaderConnected(this, EventArgs.Empty);
        }

        protected virtual void OnSmartCardReaderDisconnected()
        {
            if (SmartCardReaderDisconnected != null)
                SmartCardReaderDisconnected(this, EventArgs.Empty);
        }

        private void OpenCard()
        {
            var name = readersBox.SelectedItem as string;
            if (string.IsNullOrEmpty(name))
                return;

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            try
            {
                reader = PcscSmartCardReaderFactory.CreateDevice(name);
                reader.OpenCard();
                Program.Log(string.Format("Card Opened: {0}", reader.IsCardOpened));
                OnSmartCardReaderConnected();
            }
            catch (Exception ex)
            {
                Program.LogException(ex);
                if (reader != null)
                    reader.Dispose();
                reader = null;
            }

            RefreshUI();
        }

        private void CloseCard()
        {
            try
            {
                reader.CloseCard();
                Program.Log(string.Format("Card Opened: {0}", reader.IsCardOpened));
            }
            catch (Exception ex)
            {
                Program.LogException(ex);
            }
            finally
            {
                if (reader != null)
                    reader.Dispose();
                reader = null;
                OnSmartCardReaderDisconnected();
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            readersBox.Enabled = refreshReadersButton.Enabled = reader == null;

            openCardButton.Enabled = reader == null && readersBox.SelectedItem != null;
            closeCardButton.Enabled = reader != null;
        }

        private void FillReadersList()
        {
            readersBox.Items.Clear();
            readersBox.Items.AddRange(
                PcscSmartCardReaderFactory.EnumerateDevices());
        }
    }
}
