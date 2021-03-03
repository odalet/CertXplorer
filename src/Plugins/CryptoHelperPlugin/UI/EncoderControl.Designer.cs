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
            this.inbox = new CryptoHelperPlugin.UI.DataBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.loadButton = new System.Windows.Forms.Button();
            this.inputFormatSelector = new CryptoHelperPlugin.UI.DataFormatSelector();
            this.lineControl2 = new Delta.CertXplorer.UI.LineControl();
            this.outbox = new CryptoHelperPlugin.UI.DataBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.operationSelector = new CryptoHelperPlugin.UI.OperationSelector();
            this.lineControl4 = new Delta.CertXplorer.UI.LineControl();
            this.outputFormatSelector = new CryptoHelperPlugin.UI.DataFormatSelector();
            this.panel2 = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.lineControl3 = new Delta.CertXplorer.UI.LineControl();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.panel3.SuspendLayout();
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
            this.split.Panel1.Controls.Add(this.panel3);
            this.split.Panel1.Controls.Add(this.inputFormatSelector);
            this.split.Panel1.Controls.Add(this.lineControl2);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.outbox);
            this.split.Panel2.Controls.Add(this.panel1);
            this.split.Panel2.Controls.Add(this.panel2);
            this.split.Panel2.Controls.Add(this.lineControl3);
            this.split.Size = new System.Drawing.Size(607, 384);
            this.split.SplitterDistance = 154;
            this.split.TabIndex = 12;
            // 
            // inbox
            // 
            this.inbox.BackColor = System.Drawing.Color.White;
            this.inbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inbox.Location = new System.Drawing.Point(170, 12);
            this.inbox.Name = "inbox";
            this.inbox.Size = new System.Drawing.Size(437, 110);
            this.inbox.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.loadButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(170, 122);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(437, 32);
            this.panel3.TabIndex = 9;
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loadButton.Location = new System.Drawing.Point(0, 6);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(113, 23);
            this.loadButton.TabIndex = 8;
            this.loadButton.Text = "&Load Bytes...";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // inputFormatSelector
            // 
            this.inputFormatSelector.DataFormat = CryptoHelperPlugin.DataFormat.Text;
            this.inputFormatSelector.Dock = System.Windows.Forms.DockStyle.Left;
            this.inputFormatSelector.Location = new System.Drawing.Point(0, 12);
            this.inputFormatSelector.Name = "inputFormatSelector";
            this.inputFormatSelector.Size = new System.Drawing.Size(170, 142);
            this.inputFormatSelector.TabIndex = 1;
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
            // outbox
            // 
            this.outbox.BackColor = System.Drawing.Color.White;
            this.outbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outbox.Location = new System.Drawing.Point(170, 12);
            this.outbox.Name = "outbox";
            this.outbox.Size = new System.Drawing.Size(437, 182);
            this.outbox.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.operationSelector);
            this.panel1.Controls.Add(this.lineControl4);
            this.panel1.Controls.Add(this.outputFormatSelector);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(170, 182);
            this.panel1.TabIndex = 9;
            // 
            // operationSelector
            // 
            this.operationSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationSelector.Location = new System.Drawing.Point(0, 131);
            this.operationSelector.Name = "operationSelector";
            this.operationSelector.Operation = CryptoHelperPlugin.Operation.Convert;
            this.operationSelector.Size = new System.Drawing.Size(170, 51);
            this.operationSelector.TabIndex = 7;
            // 
            // lineControl4
            // 
            this.lineControl4.Dock = System.Windows.Forms.DockStyle.Top;
            this.lineControl4.Location = new System.Drawing.Point(0, 119);
            this.lineControl4.Name = "lineControl4";
            this.lineControl4.Padding = new System.Windows.Forms.Padding(9);
            this.lineControl4.Size = new System.Drawing.Size(170, 12);
            this.lineControl4.TabIndex = 6;
            this.lineControl4.TabStop = false;
            this.lineControl4.Text = "Operation";
            this.lineControl4.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(186)))));
            // 
            // outputFormatSelector
            // 
            this.outputFormatSelector.DataFormat = CryptoHelperPlugin.DataFormat.Text;
            this.outputFormatSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.outputFormatSelector.Location = new System.Drawing.Point(0, 0);
            this.outputFormatSelector.Name = "outputFormatSelector";
            this.outputFormatSelector.Size = new System.Drawing.Size(170, 119);
            this.outputFormatSelector.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.saveButton);
            this.panel2.Controls.Add(this.runButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 194);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(607, 32);
            this.panel2.TabIndex = 8;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveButton.Location = new System.Drawing.Point(170, 6);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(113, 23);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "&Save Bytes...";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
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
            // EncoderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.split);
            this.MinimumSize = new System.Drawing.Size(607, 365);
            this.Name = "EncoderControl";
            this.Size = new System.Drawing.Size(607, 384);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
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
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
    }
}
