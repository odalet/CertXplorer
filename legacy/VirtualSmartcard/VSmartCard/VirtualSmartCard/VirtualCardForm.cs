using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using VirtualSmartCard.DriverCom;
using System.Xml.Serialization;
using System.Xml;

namespace VirtualSmartCard
{
    public partial class VirtualCardForm : Form
    {
        ICardImplementation implementation;
        ICard card;
        public VirtualCardForm(Main mdiParent) : this(mdiParent,null) { }
        public VirtualCardForm(Main mdiParent,string fileName)
        {
            InitializeComponent();
            this.MdiParent = mdiParent;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);

            cmbReader.Items.Add("");

            foreach (var rr in Program.SimulatedReaders)
            {
                cmbReader.Items.Add(rr.Value);
            }
            cmbReader.SelectedIndex = 0;
            foreach (var v in Program.cardImplementations) {
                btnNewCard.Items.Add(v);
            }
            if (fileName != null)
                Load += new EventHandler((a,b) => { OpenFile(fileName); });
                
        }

        void card_CardInsert(bool inserted)
        {
            BeginInvoke(new Action(() =>
            {
                chkPresent.Checked = inserted;
            }));
        }
        

        void card_DriverConnect(bool connected)
        {
            BeginInvoke(new Action(() =>
            {
                chkConnected.Checked = connected;
            }));
        }

        Object queueLock = new object();
        int logQueue = 0;
        void card_Log(object msg)
        {
            if (InvokeRequired)
            {
                lock (queueLock)
                {
                    logQueue++;
                }
                BeginInvoke(new Action<object>((x) =>
                {

                    lock (queueLock)
                    {
                        logQueue--;
                    }
                    if (chkLogEnabled.Checked)
                        lstError.SelectedIndex = lstError.Items.Add(x);
                }), msg);
            }
            else
            {
                lock (queueLock)
                {
                    while (logQueue > 0)
                        Application.DoEvents();
                    lstError.SelectedIndex = lstError.Items.Add(msg);
                }
            }
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (driver!=null)
                driver.Stop();
        }

        private void bntClearLog_Click(object sender, EventArgs e)
        {
            lstError.Items.Clear();
        }



        private void btnReset_Click(object sender, EventArgs e)
        {
            if (card!=null)
                card.Handler.ResetCard(false);
        }

        private void lstError_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstError.SelectedItem != null)
            {
                Clipboard.SetText(lstError.SelectedItem.ToString());
                MessageBox.Show(lstError, "Copied on the clipboard");
            }
        }

        void SetUI(Control UIControl) {
            if (splitContainer2.Panel1.Controls.Count != 0)
                splitContainer2.Panel1.Controls.Clear();
            splitContainer2.Panel1.Controls.Add(UIControl);
            UIControl.Dock = DockStyle.Fill;
        }

        private void btnNewCard_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnNewCard.Tag != null)
                    return;
                if (btnNewCard.SelectedItem == null)
                    return;

                bool present = false;
                if (driver != null)
                {
                    present = driver.CardInserted;
                    driver.CardInserted = false;
                }
                implementation = btnNewCard.SelectedItem as ICardImplementation;

                card = implementation.GetCardObject();

                card.log += new Action<object>(card_Log);
                card.NewCard();
                SetUI(card.GetUI());

                if (driver != null)
                    driver.Handler = card.Handler;
            }
            catch (Exception ex) {
                card_Log(ex);
            }
        }

        private void resendAPDUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstError.SelectedItem != null) {
                var apdu = ByteArray.parseHex(lstError.SelectedItem.ToString());
                card.Handler.ProcessApdu(apdu);
            }
                else return;
        }

        IDriverCom driver;
        void CreateDriver(ReaderSettings settings) {
            if (settings is TcpIpReaderSettings)
            {
                var tcpReader = settings as TcpIpReaderSettings;
                driver = new SocketCom(tcpReader);
            }
            else if (settings is PipeReaderSettings)
            {
                var pipeReader = settings as PipeReaderSettings;
                driver = new PipeCom(pipeReader);
            }
            else
            {
                throw new Exception("Reader type not defined");
            }
            driver.log += new Action<object>(card_Log);
            driver.DriverConnect += new Action<bool>(card_DriverConnect);
            driver.CardInsert += new Action<bool>(card_CardInsert);
        }
        private void cmbReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReaderSettings selectedSettings = cmbReader.SelectedItem as ReaderSettings;

            if (driver == null || driver.Settings != selectedSettings)
            {
                if (driver != null)
                {
                    driver.CardInserted = false;
                    driver.Stop();
                    driver = null;
                }
                if (selectedSettings != null)
                {
                    CreateDriver(selectedSettings);

                    if (card != null)
                        driver.Handler = card.Handler;
                    driver.Start();
                }
            }
        }


        private void cmd_ApriFile(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary Card|*.bsc";
            ofd.DefaultExt = "bsc";
            if (ofd.ShowDialog(this) != DialogResult.OK)
                return;
            if (driver!=null)
                driver.Stop();
            OpenFile(ofd.FileName);
        }

        public void OpenFile(string fileName)
        {
            try
            {
                if (driver != null)
                    driver.CardInserted = false;

                FileStream fs = new FileStream(fileName, FileMode.Open);
                card = new BinaryFormatter().Deserialize(fs) as ICard;
                fs.Close();
                btnNewCard.Tag = new object();
                btnNewCard.SelectedItem = card.Implementation;
                btnNewCard.Tag = null;
                card.Name = fileName;
                Text = Path.GetFileName(card.Name);

                card.log += new Action<object>(card_Log);
                SetUI(card.GetUI());
                if (driver != null)
                    driver.Handler = card.Handler;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public void SalvaFile(object sender, EventArgs e)
        {
            if (card.Name == null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Binary Card|*.bsc";
                sfd.DefaultExt = "bsc";
                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;
                card.Name = sfd.FileName;
            }
            XmlDocument doc = new XmlDocument();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(card.Name, FileMode.Create);
            bf.Serialize(fs, card);
            fs.Close();
        }

        public void SalvaConNome(object sender, EventArgs e)
        {
            if (card != null)
            {
                string oldName = card.Name;
                card.Name = null;
                SalvaFile(this, EventArgs.Empty);
                if (card.Name == null)
                    card.Name = oldName;
            }
        }



        private void chkPresent_Click(object sender, EventArgs e)
        {
            if (driver!=null)
                driver.CardInserted = !chkPresent.Checked;
        }

        private void chkConnected_Click(object sender, EventArgs e)
        {
            if (driver != null)
            {
                if (chkConnected.Checked)
                    driver.Stop();
                else
                    driver.Start();
            }
        }

		private void btnSend_Click(object sender, EventArgs e)
		{
			try
			{
				var apdu = ByteArray.parseHex(txtAPDU.Text);
				if (card != null)
					card.Handler.ProcessApdu(apdu);
				else
					throw new Exception("Card not created");
			}
			catch (Exception ex)
			{
				card_Log(ex);
			}
			txtAPDU.Text = "";
		}
    }
}
