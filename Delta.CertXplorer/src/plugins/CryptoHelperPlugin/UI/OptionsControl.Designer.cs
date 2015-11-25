namespace CryptoHelperPlugin.UI
{
    partial class OptionsControl
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
            this.hexaConverterOptionsControl = new CryptoHelperPlugin.UI.HexaConverterOptionsControl();
            this.lineControl3 = new Delta.CertXplorer.UI.LineControl();
            this.SuspendLayout();
            // 
            // hexaConverterOptionsControl
            // 
            this.hexaConverterOptionsControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.hexaConverterOptionsControl.Location = new System.Drawing.Point(0, 12);
            this.hexaConverterOptionsControl.Name = "hexaConverterOptionsControl";
            this.hexaConverterOptionsControl.Size = new System.Drawing.Size(193, 72);
            this.hexaConverterOptionsControl.TabIndex = 0;
            // 
            // lineControl3
            // 
            this.lineControl3.Dock = System.Windows.Forms.DockStyle.Top;
            this.lineControl3.Location = new System.Drawing.Point(0, 0);
            this.lineControl3.Name = "lineControl3";
            this.lineControl3.Size = new System.Drawing.Size(193, 12);
            this.lineControl3.TabIndex = 4;
            this.lineControl3.TabStop = false;
            this.lineControl3.Text = "Hexadecimal Encoding Options";
            this.lineControl3.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(128)))), ((int)(((byte)(186)))));
            // 
            // OptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hexaConverterOptionsControl);
            this.Controls.Add(this.lineControl3);
            this.Name = "OptionsControl";
            this.Size = new System.Drawing.Size(193, 108);
            this.ResumeLayout(false);

        }

        #endregion

        private HexaConverterOptionsControl hexaConverterOptionsControl;
        private Delta.CertXplorer.UI.LineControl lineControl3;
    }
}
