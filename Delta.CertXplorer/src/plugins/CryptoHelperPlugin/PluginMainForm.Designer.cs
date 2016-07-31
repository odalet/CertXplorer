namespace CryptoHelperPlugin
{
    partial class PluginMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginMainForm));
            this.closeButton = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.encoderTab = new System.Windows.Forms.TabPage();
            this.luhnTab = new System.Windows.Forms.TabPage();
            this.oidTab = new System.Windows.Forms.TabPage();
            this.optionsTab = new System.Windows.Forms.TabPage();
            this.encoderControl = new CryptoHelperPlugin.UI.EncoderControl();
            this.luhnTesterControl = new CryptoHelperPlugin.UI.LuhnTesterControl();
            this.optionsControl = new CryptoHelperPlugin.UI.OptionsControl();
            this.oidInfoControl = new CryptoHelperPlugin.UI.OidInfoControl();
            this.tabs.SuspendLayout();
            this.encoderTab.SuspendLayout();
            this.luhnTab.SuspendLayout();
            this.oidTab.SuspendLayout();
            this.optionsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(570, 438);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.encoderTab);
            this.tabs.Controls.Add(this.luhnTab);
            this.tabs.Controls.Add(this.oidTab);
            this.tabs.Controls.Add(this.optionsTab);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(633, 420);
            this.tabs.TabIndex = 11;
            // 
            // encoderTab
            // 
            this.encoderTab.Controls.Add(this.encoderControl);
            this.encoderTab.Location = new System.Drawing.Point(4, 22);
            this.encoderTab.Name = "encoderTab";
            this.encoderTab.Padding = new System.Windows.Forms.Padding(3);
            this.encoderTab.Size = new System.Drawing.Size(625, 394);
            this.encoderTab.TabIndex = 0;
            this.encoderTab.Text = "Data Encoder/Decoder";
            this.encoderTab.UseVisualStyleBackColor = true;
            // 
            // luhnTab
            // 
            this.luhnTab.Controls.Add(this.luhnTesterControl);
            this.luhnTab.Location = new System.Drawing.Point(4, 22);
            this.luhnTab.Name = "luhnTab";
            this.luhnTab.Padding = new System.Windows.Forms.Padding(3);
            this.luhnTab.Size = new System.Drawing.Size(625, 394);
            this.luhnTab.TabIndex = 1;
            this.luhnTab.Text = "Luhn Tester";
            this.luhnTab.UseVisualStyleBackColor = true;
            // 
            // oidTab
            // 
            this.oidTab.Controls.Add(this.oidInfoControl);
            this.oidTab.Location = new System.Drawing.Point(4, 22);
            this.oidTab.Name = "oidTab";
            this.oidTab.Padding = new System.Windows.Forms.Padding(3);
            this.oidTab.Size = new System.Drawing.Size(625, 394);
            this.oidTab.TabIndex = 3;
            this.oidTab.Text = "OID Info";
            this.oidTab.UseVisualStyleBackColor = true;
            // 
            // optionsTab
            // 
            this.optionsTab.Controls.Add(this.optionsControl);
            this.optionsTab.Location = new System.Drawing.Point(4, 22);
            this.optionsTab.Name = "optionsTab";
            this.optionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.optionsTab.Size = new System.Drawing.Size(625, 394);
            this.optionsTab.TabIndex = 2;
            this.optionsTab.Text = "Options";
            this.optionsTab.UseVisualStyleBackColor = true;
            // 
            // encoderControl
            // 
            this.encoderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.encoderControl.Location = new System.Drawing.Point(3, 3);
            this.encoderControl.MinimumSize = new System.Drawing.Size(607, 365);
            this.encoderControl.Name = "encoderControl";
            this.encoderControl.Size = new System.Drawing.Size(619, 388);
            this.encoderControl.TabIndex = 0;
            // 
            // luhnTesterControl
            // 
            this.luhnTesterControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.luhnTesterControl.Location = new System.Drawing.Point(3, 3);
            this.luhnTesterControl.Name = "luhnTesterControl";
            this.luhnTesterControl.Size = new System.Drawing.Size(619, 388);
            this.luhnTesterControl.TabIndex = 0;
            // 
            // optionsControl
            // 
            this.optionsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsControl.Location = new System.Drawing.Point(3, 3);
            this.optionsControl.Name = "optionsControl";
            this.optionsControl.Size = new System.Drawing.Size(619, 388);
            this.optionsControl.TabIndex = 0;
            // 
            // oidInfoControl
            // 
            this.oidInfoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.oidInfoControl.Location = new System.Drawing.Point(3, 3);
            this.oidInfoControl.Name = "oidInfoControl";
            this.oidInfoControl.Size = new System.Drawing.Size(619, 388);
            this.oidInfoControl.TabIndex = 0;
            // 
            // PluginMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(657, 473);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.closeButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(673, 512);
            this.Name = "PluginMainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Crypto Helper";
            this.tabs.ResumeLayout(false);
            this.encoderTab.ResumeLayout(false);
            this.luhnTab.ResumeLayout(false);
            this.oidTab.ResumeLayout(false);
            this.optionsTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage encoderTab;
        private System.Windows.Forms.TabPage luhnTab;
        private UI.EncoderControl encoderControl;
        private UI.LuhnTesterControl luhnTesterControl;
        private System.Windows.Forms.TabPage optionsTab;
        private UI.OptionsControl optionsControl;
        private System.Windows.Forms.TabPage oidTab;
        private UI.OidInfoControl oidInfoControl;
    }
}