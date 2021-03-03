namespace CapiNetTestApp.UI
{
    partial class SmartCardReaderChooser
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
            this.label1 = new System.Windows.Forms.Label();
            this.readersBox = new System.Windows.Forms.ComboBox();
            this.refreshReadersButton = new System.Windows.Forms.Button();
            this.openCardButton = new System.Windows.Forms.Button();
            this.closeCardButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Smart Card Reader:";
            // 
            // readersBox
            // 
            this.readersBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.readersBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.readersBox.FormattingEnabled = true;
            this.readersBox.Location = new System.Drawing.Point(109, 5);
            this.readersBox.Name = "readersBox";
            this.readersBox.Size = new System.Drawing.Size(506, 21);
            this.readersBox.TabIndex = 1;
            // 
            // refreshReadersButton
            // 
            this.refreshReadersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshReadersButton.Location = new System.Drawing.Point(621, 3);
            this.refreshReadersButton.Name = "refreshReadersButton";
            this.refreshReadersButton.Size = new System.Drawing.Size(75, 23);
            this.refreshReadersButton.TabIndex = 2;
            this.refreshReadersButton.Text = "&Refresh";
            this.refreshReadersButton.UseVisualStyleBackColor = true;
            // 
            // openCardButton
            // 
            this.openCardButton.Location = new System.Drawing.Point(109, 32);
            this.openCardButton.Name = "openCardButton";
            this.openCardButton.Size = new System.Drawing.Size(75, 23);
            this.openCardButton.TabIndex = 2;
            this.openCardButton.Text = "&Open";
            this.openCardButton.UseVisualStyleBackColor = true;
            // 
            // closeCardButton
            // 
            this.closeCardButton.Location = new System.Drawing.Point(190, 32);
            this.closeCardButton.Name = "closeCardButton";
            this.closeCardButton.Size = new System.Drawing.Size(75, 23);
            this.closeCardButton.TabIndex = 3;
            this.closeCardButton.Text = "&Close";
            this.closeCardButton.UseVisualStyleBackColor = true;
            // 
            // SmartCardReaderChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.closeCardButton);
            this.Controls.Add(this.openCardButton);
            this.Controls.Add(this.refreshReadersButton);
            this.Controls.Add(this.readersBox);
            this.Controls.Add(this.label1);
            this.Name = "SmartCardReaderChooser";
            this.Size = new System.Drawing.Size(699, 62);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox readersBox;
        private System.Windows.Forms.Button refreshReadersButton;
        private System.Windows.Forms.Button openCardButton;
        private System.Windows.Forms.Button closeCardButton;
    }
}
