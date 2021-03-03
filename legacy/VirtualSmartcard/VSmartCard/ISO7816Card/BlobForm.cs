using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ISO7816Card
{
	public partial class BlobForm : Form
	{
		public byte[] Data;
		public BlobForm(byte[] data)
		{
			this.Data = data;
			InitializeComponent();
			txtData.Text = ByteArray.hexDump(Data,true);
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			try
			{
				Data = ByteArray.parseHex(txtData.Text);
				DialogResult = DialogResult.OK;
			}
			catch (Exception ex) {
				MessageBox.Show(this, ex.ToString());
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
