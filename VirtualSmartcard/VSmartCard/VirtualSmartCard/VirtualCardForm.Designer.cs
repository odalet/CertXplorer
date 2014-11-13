namespace VirtualSmartCard
{
    partial class VirtualCardForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VirtualCardForm));
			this.cmbReader = new System.Windows.Forms.ComboBox();
			this.lstError = new System.Windows.Forms.ListBox();
			this.commandListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.resendAPDUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chkConnected = new System.Windows.Forms.CheckBox();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.bntClearLog = new System.Windows.Forms.Button();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.btnReset = new System.Windows.Forms.Button();
			this.btnNewCard = new System.Windows.Forms.ComboBox();
			this.chkPresent = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnSend = new System.Windows.Forms.Button();
			this.txtAPDU = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.chkLogEnabled = new System.Windows.Forms.CheckBox();
			this.commandListMenu.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmbReader
			// 
			this.cmbReader.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbReader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbReader.FormattingEnabled = true;
			this.cmbReader.Location = new System.Drawing.Point(4, 4);
			this.cmbReader.Name = "cmbReader";
			this.cmbReader.Size = new System.Drawing.Size(635, 21);
			this.cmbReader.TabIndex = 0;
			this.cmbReader.SelectedIndexChanged += new System.EventHandler(this.cmbReader_SelectedIndexChanged);
			// 
			// lstError
			// 
			this.lstError.ContextMenuStrip = this.commandListMenu;
			this.lstError.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstError.FormattingEnabled = true;
			this.lstError.IntegralHeight = false;
			this.lstError.Location = new System.Drawing.Point(0, 0);
			this.lstError.Name = "lstError";
			this.lstError.Size = new System.Drawing.Size(635, 75);
			this.lstError.TabIndex = 2;
			this.lstError.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstError_MouseDoubleClick);
			// 
			// commandListMenu
			// 
			this.commandListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resendAPDUToolStripMenuItem});
			this.commandListMenu.Name = "commandListMenu";
			this.commandListMenu.Size = new System.Drawing.Size(147, 26);
			// 
			// resendAPDUToolStripMenuItem
			// 
			this.resendAPDUToolStripMenuItem.Name = "resendAPDUToolStripMenuItem";
			this.resendAPDUToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
			this.resendAPDUToolStripMenuItem.Text = "Resend APDU";
			this.resendAPDUToolStripMenuItem.Click += new System.EventHandler(this.resendAPDUToolStripMenuItem_Click);
			// 
			// chkConnected
			// 
			this.chkConnected.AutoCheck = false;
			this.chkConnected.AutoSize = true;
			this.chkConnected.Location = new System.Drawing.Point(6, 3);
			this.chkConnected.Name = "chkConnected";
			this.chkConnected.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.chkConnected.Size = new System.Drawing.Size(109, 20);
			this.chkConnected.TabIndex = 3;
			this.chkConnected.Text = "Driver Connected";
			this.chkConnected.UseVisualStyleBackColor = true;
			this.chkConnected.Click += new System.EventHandler(this.chkConnected_Click);
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(4, 125);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.lstError);
			this.splitContainer2.Panel2.Controls.Add(this.bntClearLog);
			this.splitContainer2.Panel2.Controls.Add(this.statusStrip1);
			this.splitContainer2.Size = new System.Drawing.Size(635, 344);
			this.splitContainer2.SplitterDistance = 216;
			this.splitContainer2.SplitterWidth = 8;
			this.splitContainer2.TabIndex = 4;
			// 
			// bntClearLog
			// 
			this.bntClearLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bntClearLog.Location = new System.Drawing.Point(0, 75);
			this.bntClearLog.Name = "bntClearLog";
			this.bntClearLog.Size = new System.Drawing.Size(635, 23);
			this.bntClearLog.TabIndex = 3;
			this.bntClearLog.Text = "Clear Log";
			this.bntClearLog.UseVisualStyleBackColor = true;
			this.bntClearLog.Click += new System.EventHandler(this.bntClearLog_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 98);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(635, 22);
			this.statusStrip1.TabIndex = 4;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// btnReset
			// 
			this.btnReset.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnReset.Location = new System.Drawing.Point(4, 46);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(635, 23);
			this.btnReset.TabIndex = 5;
			this.btnReset.Text = "Reset Card";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// btnNewCard
			// 
			this.btnNewCard.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnNewCard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.btnNewCard.Location = new System.Drawing.Point(4, 25);
			this.btnNewCard.Name = "btnNewCard";
			this.btnNewCard.Size = new System.Drawing.Size(635, 21);
			this.btnNewCard.TabIndex = 6;
			this.btnNewCard.SelectedIndexChanged += new System.EventHandler(this.btnNewCard_Click);
			// 
			// chkPresent
			// 
			this.chkPresent.AutoCheck = false;
			this.chkPresent.AutoSize = true;
			this.chkPresent.Location = new System.Drawing.Point(121, 3);
			this.chkPresent.Name = "chkPresent";
			this.chkPresent.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.chkPresent.Size = new System.Drawing.Size(87, 20);
			this.chkPresent.TabIndex = 9;
			this.chkPresent.Text = "Card Present";
			this.chkPresent.UseVisualStyleBackColor = true;
			this.chkPresent.Click += new System.EventHandler(this.chkPresent_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnSend);
			this.panel1.Controls.Add(this.txtAPDU);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.chkLogEnabled);
			this.panel1.Controls.Add(this.chkConnected);
			this.panel1.Controls.Add(this.chkPresent);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(4, 69);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(635, 56);
			this.panel1.TabIndex = 10;
			// 
			// btnSend
			// 
			this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSend.Location = new System.Drawing.Point(574, 29);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(58, 21);
			this.btnSend.TabIndex = 13;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// txtAPDU
			// 
			this.txtAPDU.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtAPDU.Location = new System.Drawing.Point(46, 29);
			this.txtAPDU.Name = "txtAPDU";
			this.txtAPDU.Size = new System.Drawing.Size(522, 20);
			this.txtAPDU.TabIndex = 12;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "APDU";
			// 
			// chkLogEnabled
			// 
			this.chkLogEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chkLogEnabled.AutoSize = true;
			this.chkLogEnabled.Checked = true;
			this.chkLogEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkLogEnabled.Location = new System.Drawing.Point(549, 3);
			this.chkLogEnabled.Name = "chkLogEnabled";
			this.chkLogEnabled.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.chkLogEnabled.Size = new System.Drawing.Size(86, 20);
			this.chkLogEnabled.TabIndex = 10;
			this.chkLogEnabled.Text = "Log Enabled";
			this.chkLogEnabled.UseVisualStyleBackColor = true;
			// 
			// VirtualCardForm
			// 
			this.AcceptButton = this.btnSend;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(643, 473);
			this.Controls.Add(this.splitContainer2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.btnNewCard);
			this.Controls.Add(this.cmbReader);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "VirtualCardForm";
			this.Padding = new System.Windows.Forms.Padding(4);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Smart Card";
			this.commandListMenu.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			this.splitContainer2.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbReader;
        private System.Windows.Forms.ListBox lstError;
        private System.Windows.Forms.CheckBox chkConnected;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button bntClearLog;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ComboBox btnNewCard;
        private System.Windows.Forms.ContextMenuStrip commandListMenu;
        private System.Windows.Forms.ToolStripMenuItem resendAPDUToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.CheckBox chkPresent;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkLogEnabled;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.TextBox txtAPDU;
		private System.Windows.Forms.Label label1;
    }
}

