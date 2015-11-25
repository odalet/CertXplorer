namespace CryptoHelperPlugin.UI
{
    partial class HexaConverterOptionsControl
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
            this.prefixCheckBox = new System.Windows.Forms.CheckBox();
            this.lowerCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.separatorBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // prefixCheckBox
            // 
            this.prefixCheckBox.AutoSize = true;
            this.prefixCheckBox.Location = new System.Drawing.Point(65, 29);
            this.prefixCheckBox.Name = "prefixCheckBox";
            this.prefixCheckBox.Size = new System.Drawing.Size(88, 17);
            this.prefixCheckBox.TabIndex = 0;
            this.prefixCheckBox.Text = "Prefix with 0x";
            this.prefixCheckBox.UseVisualStyleBackColor = true;
            this.prefixCheckBox.CheckedChanged += new System.EventHandler(this.prefixCheckBox_CheckedChanged);
            // 
            // lowerCaseCheckBox
            // 
            this.lowerCaseCheckBox.AutoSize = true;
            this.lowerCaseCheckBox.Location = new System.Drawing.Point(65, 52);
            this.lowerCaseCheckBox.Name = "lowerCaseCheckBox";
            this.lowerCaseCheckBox.Size = new System.Drawing.Size(82, 17);
            this.lowerCaseCheckBox.TabIndex = 0;
            this.lowerCaseCheckBox.Text = "Lower Case";
            this.lowerCaseCheckBox.UseVisualStyleBackColor = true;
            this.lowerCaseCheckBox.CheckedChanged += new System.EventHandler(this.lowerCaseCheckBox_CheckedChanged);
            // 
            // separatorBox
            // 
            this.separatorBox.Location = new System.Drawing.Point(65, 3);
            this.separatorBox.Name = "separatorBox";
            this.separatorBox.Size = new System.Drawing.Size(100, 20);
            this.separatorBox.TabIndex = 1;
            this.separatorBox.TextChanged += new System.EventHandler(this.separatorBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Separator:";
            // 
            // HexaConverterOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.separatorBox);
            this.Controls.Add(this.lowerCaseCheckBox);
            this.Controls.Add(this.prefixCheckBox);
            this.Name = "HexaConverterOptionsControl";
            this.Size = new System.Drawing.Size(172, 72);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox prefixCheckBox;
        private System.Windows.Forms.CheckBox lowerCaseCheckBox;
        private System.Windows.Forms.TextBox separatorBox;
        private System.Windows.Forms.Label label1;
    }
}
