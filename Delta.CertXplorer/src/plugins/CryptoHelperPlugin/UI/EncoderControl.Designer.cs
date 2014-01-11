namespace CryptoHelperPlugin.UI
{
    partial class EncoderControl
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
            this.split = new System.Windows.Forms.SplitContainer();
            this.lineControl2 = new Delta.CertXplorer.UI.LineControl();
            this.runButton = new System.Windows.Forms.Button();
            this.lineControl3 = new Delta.CertXplorer.UI.LineControl();
            this.lineControl4 = new Delta.CertXplorer.UI.LineControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.inbox = new CryptoHelperPlugin.UI.DataBox();
            this.inputFormatSelector = new CryptoHelperPlugin.UI.DataFormatSelector();
            this.outbox = new CryptoHelperPlugin.UI.DataBox();
            this.operationSelector = new CryptoHelperPlugin.UI.OperationSelector();
            this.outputFormatSelector = new CryptoHelperPlugin.UI.DataFormatSelector();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // split
            // 
            this.split.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Location = new System.Drawing.Point(0, 0);
            this.split.Margin = new System.Windows.Forms.Padding(0);
            this.split.Name = "split";
            this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.inbox);
            this.split.Panel1.Controls.Add(this.inputFormatSelector);
            this.split.Panel1.Controls.Add(this.lineControl2);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.outbox);
            this.split.Panel2.Controls.Add(this.panel1);
            this.split.Panel2.Controls.Add(this.lineControl3);
            this.split.Size = new System.Drawing.Size(607, 365);
            this.split.SplitterDistance = 104;
            this.split.TabIndex = 12;
            // 
            // lineControl2
            // 
            this.lineControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.lineControl2.Location = new System.Drawing.Point(0, 0);
            this.lineControl2.Name = "lineControl2";
            this.lineControl2.Size = new System.Drawing.Size(607, 12);
            this.lineControl2.TabIndex = 0;
            this.lineControl2.TabStop = false;
            this.lineControl2.Text = "Input";
            this.lineControl2.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(186)))));
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.runButton.Location = new System.Drawing.Point(3, 6);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 8;
            this.runButton.Text = "&Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // lineControl3
            // 
            this.lineControl3.Dock = System.Windows.Forms.DockStyle.Top;
            this.lineControl3.Location = new System.Drawing.Point(0, 0);
            this.lineControl3.Name = "lineControl3";
            this.lineControl3.Size = new System.Drawing.Size(607, 12);
            this.lineControl3.TabIndex = 3;
            this.lineControl3.TabStop = false;
            this.lineControl3.Text = "Output";
            this.lineControl3.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(186)))));
            // 
            // lineControl4
            // 
            this.lineControl4.Dock = System.Windows.Forms.DockStyle.Top;
            this.lineControl4.Location = new System.Drawing.Point(0, 99);
            this.lineControl4.Name = "lineControl4";
            this.lineControl4.Padding = new System.Windows.Forms.Padding(9);
            this.lineControl4.Size = new System.Drawing.Size(170, 12);
            this.lineControl4.TabIndex = 6;
            this.lineControl4.TabStop = false;
            this.lineControl4.Text = "Operation";
            this.lineControl4.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(186)))));
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.operationSelector);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lineControl4);
            this.panel1.Controls.Add(this.outputFormatSelector);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(170, 245);
            this.panel1.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.runButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 213);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(170, 32);
            this.panel2.TabIndex = 8;
            // 
            // inbox
            // 
            this.inbox.BackColor = System.Drawing.Color.White;
            this.inbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inbox.Location = new System.Drawing.Point(170, 12);
            this.inbox.Name = "inbox";
            this.inbox.Size = new System.Drawing.Size(437, 92);
            this.inbox.TabIndex = 2;
            // 
            // inputFormatSelector
            // 
            this.inputFormatSelector.DataFormat = CryptoHelperPlugin.DataFormat.Text;
            this.inputFormatSelector.Dock = System.Windows.Forms.DockStyle.Left;
            this.inputFormatSelector.Location = new System.Drawing.Point(0, 12);
            this.inputFormatSelector.Name = "inputFormatSelector";
            this.inputFormatSelector.Size = new System.Drawing.Size(170, 92);
            this.inputFormatSelector.TabIndex = 1;
            // 
            // outbox
            // 
            this.outbox.BackColor = System.Drawing.Color.White;
            this.outbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outbox.Location = new System.Drawing.Point(170, 12);
            this.outbox.Name = "outbox";
            this.outbox.Size = new System.Drawing.Size(437, 245);
            this.outbox.TabIndex = 5;
            // 
            // operationSelector
            // 
            this.operationSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationSelector.Location = new System.Drawing.Point(0, 111);
            this.operationSelector.Name = "operationSelector";
            this.operationSelector.Operation = CryptoHelperPlugin.Operation.Convert;
            this.operationSelector.Size = new System.Drawing.Size(170, 102);
            this.operationSelector.TabIndex = 7;
            // 
            // outputFormatSelector
            // 
            this.outputFormatSelector.DataFormat = CryptoHelperPlugin.DataFormat.Text;
            this.outputFormatSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.outputFormatSelector.Location = new System.Drawing.Point(0, 0);
            this.outputFormatSelector.Name = "outputFormatSelector";
            this.outputFormatSelector.Size = new System.Drawing.Size(170, 99);
            this.outputFormatSelector.TabIndex = 4;
            // 
            // EncoderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.split);
            this.MinimumSize = new System.Drawing.Size(607, 365);
            this.Name = "EncoderControl";
            this.Size = new System.Drawing.Size(607, 365);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer split;
        private DataBox inbox;
        private DataFormatSelector inputFormatSelector;
        private Delta.CertXplorer.UI.LineControl lineControl2;
        private OperationSelector operationSelector;
        private System.Windows.Forms.Button runButton;
        private DataFormatSelector outputFormatSelector;
        private Delta.CertXplorer.UI.LineControl lineControl3;
        private DataBox outbox;
        private Delta.CertXplorer.UI.LineControl lineControl4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;

    }
}
