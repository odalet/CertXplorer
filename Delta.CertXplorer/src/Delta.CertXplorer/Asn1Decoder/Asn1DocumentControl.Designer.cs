﻿namespace Delta.CertXplorer.Asn1Decoder
{
    partial class Asn1DocumentControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Asn1DocumentControl));
            this.tstrip = new System.Windows.Forms.ToolStrip();
            this.refreshToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.parseOctetStringsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.showInvalidTaggedObjectsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.parseAsDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.standardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.icaoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewer = new Delta.CertXplorer.Asn1Decoder.Asn1Viewer();
            this.tstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tstrip
            // 
            this.tstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripButton,
            this.parseOctetStringsToolStripButton,
            this.showInvalidTaggedObjectsToolStripButton,
            this.parseAsDropDownButton});
            this.tstrip.Location = new System.Drawing.Point(0, 0);
            this.tstrip.Name = "tstrip";
            this.tstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.tstrip.Size = new System.Drawing.Size(682, 25);
            this.tstrip.TabIndex = 2;
            this.tstrip.Text = "toolStrip1";
            // 
            // refreshToolStripButton
            // 
            this.refreshToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshToolStripButton.Image = global::Delta.CertXplorer.Properties.Resources.Refresh;
            this.refreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.refreshToolStripButton.Name = "refreshToolStripButton";
            this.refreshToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.refreshToolStripButton.Text = "Refresh";
            // 
            // parseOctetStringsToolStripButton
            // 
            this.parseOctetStringsToolStripButton.CheckOnClick = true;
            this.parseOctetStringsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.parseOctetStringsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("parseOctetStringsToolStripButton.Image")));
            this.parseOctetStringsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.parseOctetStringsToolStripButton.Name = "parseOctetStringsToolStripButton";
            this.parseOctetStringsToolStripButton.Size = new System.Drawing.Size(110, 22);
            this.parseOctetStringsToolStripButton.Text = "&Parse Octet Strings";
            // 
            // showInvalidTaggedObjectsToolStripButton
            // 
            this.showInvalidTaggedObjectsToolStripButton.CheckOnClick = true;
            this.showInvalidTaggedObjectsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.showInvalidTaggedObjectsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("showInvalidTaggedObjectsToolStripButton.Image")));
            this.showInvalidTaggedObjectsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showInvalidTaggedObjectsToolStripButton.Name = "showInvalidTaggedObjectsToolStripButton";
            this.showInvalidTaggedObjectsToolStripButton.Size = new System.Drawing.Size(163, 22);
            this.showInvalidTaggedObjectsToolStripButton.Text = "&Show Invalid Tagged Objects";
            // 
            // parseAsDropDownButton
            // 
            this.parseAsDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.parseAsDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.standardToolStripMenuItem,
            this.icaoToolStripMenuItem,
            this.cvToolStripMenuItem});
            this.parseAsDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("parseAsDropDownButton.Image")));
            this.parseAsDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.parseAsDropDownButton.Name = "parseAsDropDownButton";
            this.parseAsDropDownButton.Size = new System.Drawing.Size(73, 22);
            this.parseAsDropDownButton.Text = "Parse &As...";
            // 
            // standardToolStripMenuItem
            // 
            this.standardToolStripMenuItem.CheckOnClick = true;
            this.standardToolStripMenuItem.Name = "standardToolStripMenuItem";
            this.standardToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.standardToolStripMenuItem.Text = "Standard &ASN.1";
            // 
            // icaoToolStripMenuItem
            // 
            this.icaoToolStripMenuItem.CheckOnClick = true;
            this.icaoToolStripMenuItem.Name = "icaoToolStripMenuItem";
            this.icaoToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.icaoToolStripMenuItem.Text = "&ICAO MRTD";
            // 
            // cvToolStripMenuItem
            // 
            this.cvToolStripMenuItem.CheckOnClick = true;
            this.cvToolStripMenuItem.Name = "cvToolStripMenuItem";
            this.cvToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.cvToolStripMenuItem.Text = "&Card Verifiable";
            // 
            // viewer
            // 
            this.viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewer.Location = new System.Drawing.Point(0, 25);
            this.viewer.Name = "viewer";
            this.viewer.Size = new System.Drawing.Size(682, 601);
            this.viewer.TabIndex = 3;
            // 
            // Asn1DocumentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.viewer);
            this.Controls.Add(this.tstrip);
            this.Name = "Asn1DocumentControl";
            this.Size = new System.Drawing.Size(682, 626);
            this.tstrip.ResumeLayout(false);
            this.tstrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tstrip;
        private System.Windows.Forms.ToolStripButton refreshToolStripButton;
        private System.Windows.Forms.ToolStripButton parseOctetStringsToolStripButton;
        private Asn1Viewer viewer;
        private System.Windows.Forms.ToolStripButton showInvalidTaggedObjectsToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton parseAsDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem standardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem icaoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cvToolStripMenuItem;
    }
}
