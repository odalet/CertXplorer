using System;
using System.Windows.Forms;

namespace CryptoHelperPlugin.UI
{
    public partial class HexaConverterOptionsControl : UserControl
    {
        private bool initializing = false;

        public HexaConverterOptionsControl()
        {
            InitializeComponent();            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            initializing = true;

            prefixCheckBox.Checked = HexaConverter.ConverterOptions.PrefixWithZeroX;
            lowerCaseCheckBox.Checked = HexaConverter.ConverterOptions.LowerCase;
            separatorBox.Text = HexaConverter.ConverterOptions.Separator ?? string.Empty;

            initializing = false;
        }

        private void prefixCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (initializing) return;
            HexaConverter.ConverterOptions.PrefixWithZeroX = prefixCheckBox.Checked;
        }

        private void lowerCaseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (initializing) return;
            HexaConverter.ConverterOptions.LowerCase = lowerCaseCheckBox.Checked;
        }

        private void separatorBox_TextChanged(object sender, EventArgs e)
        {
            if (initializing) return;
            HexaConverter.ConverterOptions.Separator = separatorBox.Text ?? string.Empty;

        }
    }
}
