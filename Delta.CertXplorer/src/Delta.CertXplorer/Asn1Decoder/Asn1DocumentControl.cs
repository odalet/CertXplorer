using System;
using System.Windows.Forms;
using System.ComponentModel;

using Delta.CertXplorer.UI.Theming;
using Delta.CertXplorer.DocumentModel;

namespace Delta.CertXplorer.Asn1Decoder
{
    internal partial class Asn1DocumentControl : UserControl
    {
        private bool eventsSuspended = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Asn1DocumentControl"/> class.
        /// </summary>
        public Asn1DocumentControl()
        {
            InitializeComponent();

            ThemesManager.RegisterThemeAwareControl(this, (renderer) =>
            {
                if (renderer is ToolStripProfessionalRenderer)
                    ((ToolStripProfessionalRenderer)renderer).RoundedEdges = false;
                tstrip.Renderer = renderer;
            });

            tstrip.SetRoundedEdges(false);
        }

        public void SetData(byte[] bytes)
        {
            var data = bytes;

            // ASN.1 viewer doesn't support (yet) null data!
            if (data == null) data = new byte[0];
            viewer.Initialize(data);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            refreshToolStripButton.Click += (s, _) => viewer.ParseData();

            parseOctetStringsToolStripButton.Checked = viewer.ParseOctetStrings;
            parseOctetStringsToolStripButton.CheckedChanged += (s, _) =>
            {
                viewer.ParseOctetStrings = parseOctetStringsToolStripButton.Checked;
                viewer.ParseData();
            };

            showInvalidTaggedObjectsToolStripButton.Checked = viewer.ShowInvalidTaggedObjects;
            showInvalidTaggedObjectsToolStripButton.CheckedChanged += (s, _) =>
            {
                viewer.ShowInvalidTaggedObjects = showInvalidTaggedObjectsToolStripButton.Checked;
                viewer.ParseData();
            };

            // Initialize parse mode

            icaoToolStripMenuItem.Checked = false;
            cvToolStripMenuItem.Checked = false;
            standardToolStripMenuItem.Checked = false;

            switch (viewer.ParseMode)
            {
                case Asn1ParseMode.Icao: icaoToolStripMenuItem.Checked = true; break;
                case Asn1ParseMode.CardVerifiable: cvToolStripMenuItem.Checked = true; break;
                default: standardToolStripMenuItem.Checked = true; break;
            }

            icaoToolStripMenuItem.CheckedChanged += (s, _) => UpdateParseMode(Asn1ParseMode.Icao);
            cvToolStripMenuItem.CheckedChanged += (s, _) => UpdateParseMode(Asn1ParseMode.CardVerifiable);
            standardToolStripMenuItem.CheckedChanged += (s, _) => UpdateParseMode(Asn1ParseMode.Standard);            
        }

        private void UpdateParseMode(Asn1ParseMode mode)
        {
            if (eventsSuspended) return;
            eventsSuspended = true;

            icaoToolStripMenuItem.Checked = false;
            cvToolStripMenuItem.Checked = false;
            standardToolStripMenuItem.Checked = false;

            switch (mode)
            {
                case Asn1ParseMode.Icao: icaoToolStripMenuItem.Checked = true; break;
                case Asn1ParseMode.CardVerifiable: cvToolStripMenuItem.Checked = true; break;
                default: standardToolStripMenuItem.Checked = true; break;
            }

            eventsSuspended = false;

            viewer.ParseMode = mode;
            viewer.ParseData();
        }
    }
}
