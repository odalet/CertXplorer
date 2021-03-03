namespace Delta.CertXplorer.UI
{
    partial class BaseChrome
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.bottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.leftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.rightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.workspace = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.topToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.actionsManager = new Delta.CertXplorer.UI.Actions.UIActionsManager();
            this.vS2015BlueTheme = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();
            ((System.ComponentModel.ISupportInitialize)(this.actionsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(604, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 469);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(604, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            // 
            // bottomToolStripPanel
            // 
            this.bottomToolStripPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomToolStripPanel.Location = new System.Drawing.Point(0, 469);
            this.bottomToolStripPanel.Name = "bottomToolStripPanel";
            this.bottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.bottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.bottomToolStripPanel.Size = new System.Drawing.Size(604, 0);
            // 
            // leftToolStripPanel
            // 
            this.leftToolStripPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftToolStripPanel.Location = new System.Drawing.Point(0, 24);
            this.leftToolStripPanel.Name = "leftToolStripPanel";
            this.leftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.leftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.leftToolStripPanel.Size = new System.Drawing.Size(0, 445);
            // 
            // rightToolStripPanel
            // 
            this.rightToolStripPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightToolStripPanel.Location = new System.Drawing.Point(604, 24);
            this.rightToolStripPanel.Name = "rightToolStripPanel";
            this.rightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.rightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.rightToolStripPanel.Size = new System.Drawing.Size(0, 445);
            // 
            // workspace
            // 
            this.workspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workspace.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(57)))), ((int)(((byte)(85)))));
            this.workspace.Location = new System.Drawing.Point(0, 24);
            this.workspace.Name = "workspace";
            this.workspace.Padding = new System.Windows.Forms.Padding(6);
            this.workspace.ShowAutoHideContentOnHover = false;
            this.workspace.Size = new System.Drawing.Size(604, 445);
            this.workspace.TabIndex = 5;
            this.workspace.Theme = this.vS2015BlueTheme;
            // 
            // topToolStripPanel
            // 
            this.topToolStripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topToolStripPanel.Location = new System.Drawing.Point(0, 24);
            this.topToolStripPanel.Name = "topToolStripPanel";
            this.topToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.topToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.topToolStripPanel.Size = new System.Drawing.Size(604, 0);
            // 
            // actionsManager
            // 
            this.actionsManager.ContainerControl = this;
            // 
            // BaseChrome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 491);
            this.Controls.Add(this.workspace);
            this.Controls.Add(this.topToolStripPanel);
            this.Controls.Add(this.leftToolStripPanel);
            this.Controls.Add(this.rightToolStripPanel);
            this.Controls.Add(this.bottomToolStripPanel);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "BaseChrome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.actionsManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        protected WeifenLuo.WinFormsUI.Docking.DockPanel workspace;
        protected System.Windows.Forms.ToolStripPanel topToolStripPanel;
        protected System.Windows.Forms.ToolStripPanel bottomToolStripPanel;
        protected System.Windows.Forms.ToolStripPanel leftToolStripPanel;
        protected System.Windows.Forms.ToolStripPanel rightToolStripPanel;
        private Delta.CertXplorer.UI.Actions.UIActionsManager actionsManager;
        private System.Windows.Forms.MenuStrip menuStrip;
        private WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme vS2015BlueTheme;
    }
}

