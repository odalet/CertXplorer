namespace TestCapiNet.UI
{
    partial class LogBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogBox));
            this.tstrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.rtb = new System.Windows.Forms.RichTextBox();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.tstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tstrip
            // 
            this.tstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
            this.tstrip.Location = new System.Drawing.Point(0, 0);
            this.tstrip.Name = "tstrip";
            this.tstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tstrip.Size = new System.Drawing.Size(490, 25);
            this.tstrip.TabIndex = 0;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(38, 22);
            this.toolStripButton1.Text = "C&lear";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // rtb
            // 
            this.rtb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtb.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb.Location = new System.Drawing.Point(0, 25);
            this.rtb.Name = "rtb";
            this.rtb.ReadOnly = true;
            this.rtb.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.rtb.Size = new System.Drawing.Size(490, 100);
            this.rtb.TabIndex = 1;
            this.rtb.Text = "";
            this.rtb.WordWrap = false;
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(39, 22);
            this.toolStripButton2.Text = "&Copy";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // LogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtb);
            this.Controls.Add(this.tstrip);
            this.Name = "LogBox";
            this.Size = new System.Drawing.Size(490, 125);
            this.tstrip.ResumeLayout(false);
            this.tstrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tstrip;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.RichTextBox rtb;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}
