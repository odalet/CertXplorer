
namespace Delta.CertXplorer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.workspace = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vs2015BlueTheme = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCertificateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.topToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.openFileToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openCertificateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.aboutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.leftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.rightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.bottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.actionsManager = new Delta.CertXplorer.UI.Actions.UIActionsManager();
            this.openFileDocumentAction = new Delta.CertXplorer.UI.Actions.UIAction();
            this.openCertificateDocumentAction = new Delta.CertXplorer.UI.Actions.UIAction();
            this.exitAction = new Delta.CertXplorer.UI.Actions.UIAction();
            this.aboutAction = new Delta.CertXplorer.UI.Actions.UIAction();
            this.menuStrip.SuspendLayout();
            this.topToolStripPanel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actionsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // workspace
            // 
            this.workspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workspace.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(57)))), ((int)(((byte)(85)))));
            this.workspace.Location = new System.Drawing.Point(0, 49);
            this.workspace.Name = "workspace";
            this.workspace.Padding = new System.Windows.Forms.Padding(6);
            this.workspace.ShowAutoHideContentOnHover = false;
            this.workspace.Size = new System.Drawing.Size(604, 420);
            this.workspace.TabIndex = 6;
            this.workspace.Theme = this.vs2015BlueTheme;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(604, 24);
            this.menuStrip.TabIndex = 7;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.openCertificateToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openFileToolStripMenuItem.Image")));
            this.openFileToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.openFileToolStripMenuItem.Text = "&Open File…";
            // 
            // openCertificateToolStripMenuItem
            // 
            this.openCertificateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openCertificateToolStripMenuItem.Image")));
            this.openCertificateToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.openCertificateToolStripMenuItem.Name = "openCertificateToolStripMenuItem";
            this.openCertificateToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.openCertificateToolStripMenuItem.Text = "&Open Certificate…";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(166, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About…";
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 469);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(604, 22);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // topToolStripPanel
            // 
            this.topToolStripPanel.Controls.Add(this.toolStrip);
            this.topToolStripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topToolStripPanel.Location = new System.Drawing.Point(0, 24);
            this.topToolStripPanel.Name = "topToolStripPanel";
            this.topToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.topToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.topToolStripPanel.Size = new System.Drawing.Size(604, 25);
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripButton,
            this.openCertificateToolStripButton,
            this.aboutToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(3, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(81, 25);
            this.toolStrip.TabIndex = 12;
            this.toolStrip.Text = "toolStrip1";
            // 
            // openFileToolStripButton
            // 
            this.openFileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFileToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openFileToolStripButton.Image")));
            this.openFileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFileToolStripButton.Name = "openFileToolStripButton";
            this.openFileToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openFileToolStripButton.Text = "&Open File";
            this.openFileToolStripButton.ToolTipText = "Open File…";
            // 
            // openCertificateToolStripButton
            // 
            this.openCertificateToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openCertificateToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openCertificateToolStripButton.Image")));
            this.openCertificateToolStripButton.ImageTransparentColor = System.Drawing.Color.White;
            this.openCertificateToolStripButton.Name = "openCertificateToolStripButton";
            this.openCertificateToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openCertificateToolStripButton.Text = "Open Certificate…";
            // 
            // aboutToolStripButton
            // 
            this.aboutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.aboutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripButton.Image")));
            this.aboutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.aboutToolStripButton.Name = "aboutToolStripButton";
            this.aboutToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.aboutToolStripButton.Text = "&About…";
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
            // bottomToolStripPanel
            // 
            this.bottomToolStripPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomToolStripPanel.Location = new System.Drawing.Point(0, 469);
            this.bottomToolStripPanel.Name = "bottomToolStripPanel";
            this.bottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.bottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.bottomToolStripPanel.Size = new System.Drawing.Size(604, 0);
            // 
            // actionsManager
            // 
            this.actionsManager.Actions.Add(this.openFileDocumentAction);
            this.actionsManager.Actions.Add(this.openCertificateDocumentAction);
            this.actionsManager.Actions.Add(this.exitAction);
            this.actionsManager.Actions.Add(this.aboutAction);
            this.actionsManager.ContainerControl = this;
            // 
            // MainForm
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.topToolStripPanel.ResumeLayout(false);
            this.topToolStripPanel.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actionsManager)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected WeifenLuo.WinFormsUI.Docking.DockPanel workspace;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        protected System.Windows.Forms.ToolStripPanel topToolStripPanel;
        protected System.Windows.Forms.ToolStripPanel leftToolStripPanel;
        protected System.Windows.Forms.ToolStripPanel rightToolStripPanel;
        protected System.Windows.Forms.ToolStripPanel bottomToolStripPanel;
        private UI.Actions.UIActionsManager actionsManager;
        private WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme vs2015BlueTheme;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCertificateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private UI.Actions.UIAction openFileDocumentAction;
        private UI.Actions.UIAction exitAction;
        private UI.Actions.UIAction openCertificateDocumentAction;
        private UI.Actions.UIAction aboutAction;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton openFileToolStripButton;
        private System.Windows.Forms.ToolStripButton openCertificateToolStripButton;
        private System.Windows.Forms.ToolStripButton aboutToolStripButton;
    }
}