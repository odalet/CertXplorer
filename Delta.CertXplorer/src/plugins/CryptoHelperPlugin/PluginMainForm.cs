using System;
using System.Windows.Forms;

namespace CryptoHelperPlugin
{
    internal partial class PluginMainForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginMainForm"/> class.
        /// </summary>
        public PluginMainForm()
        {
            InitializeComponent();
        }

        public Plugin Plugin { get; set; }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
