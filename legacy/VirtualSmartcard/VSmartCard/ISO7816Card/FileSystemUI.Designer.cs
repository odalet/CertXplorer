namespace CardModule
{
    partial class FileSystemUI
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileSystemUI));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.FileSystemMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.creaEFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creaEFTLVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creaDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creaBSOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creaSEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.eliminaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copiaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iNcollaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cardImageList = new System.Windows.Forms.ImageList(this.components);
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.FileSystemMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.splitContainer1.Size = new System.Drawing.Size(546, 352);
            this.splitContainer1.SplitterDistance = 179;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.FileSystemMenu;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.cardImageList;
            this.treeView1.Location = new System.Drawing.Point(0, 5);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(179, 347);
            this.treeView1.TabIndex = 0;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // FileSystemMenu
            // 
            this.FileSystemMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.creaEFToolStripMenuItem,
            this.creaEFTLVToolStripMenuItem,
            this.creaDFToolStripMenuItem,
            this.creaBSOToolStripMenuItem,
            this.creaSEToolStripMenuItem,
            this.toolStripSeparator1,
            this.eliminaToolStripMenuItem,
            this.copiaToolStripMenuItem,
            this.iNcollaToolStripMenuItem});
            this.FileSystemMenu.Name = "FileSystemMenu";
            this.FileSystemMenu.Size = new System.Drawing.Size(172, 186);
            // 
            // creaEFToolStripMenuItem
            // 
            this.creaEFToolStripMenuItem.Name = "creaEFToolStripMenuItem";
            this.creaEFToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.creaEFToolStripMenuItem.Text = "Crea EF binary";
            this.creaEFToolStripMenuItem.Click += new System.EventHandler(this.CreaEF);
            // 
            // creaEFTLVToolStripMenuItem
            // 
            this.creaEFTLVToolStripMenuItem.Name = "creaEFTLVToolStripMenuItem";
            this.creaEFTLVToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.creaEFTLVToolStripMenuItem.Text = "Crea EF Linear TLV";
            this.creaEFTLVToolStripMenuItem.Click += new System.EventHandler(this.creaEFTLVToolStripMenuItem_Click);
            // 
            // creaDFToolStripMenuItem
            // 
            this.creaDFToolStripMenuItem.Name = "creaDFToolStripMenuItem";
            this.creaDFToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.creaDFToolStripMenuItem.Text = "Crea DF";
            this.creaDFToolStripMenuItem.Click += new System.EventHandler(this.CreaDF);
            // 
            // creaBSOToolStripMenuItem
            // 
            this.creaBSOToolStripMenuItem.Name = "creaBSOToolStripMenuItem";
            this.creaBSOToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.creaBSOToolStripMenuItem.Text = "Crea BSO";
            this.creaBSOToolStripMenuItem.Click += new System.EventHandler(this.CreaBSO);
            // 
            // creaSEToolStripMenuItem
            // 
            this.creaSEToolStripMenuItem.Name = "creaSEToolStripMenuItem";
            this.creaSEToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.creaSEToolStripMenuItem.Text = "Crea SE";
            this.creaSEToolStripMenuItem.Click += new System.EventHandler(this.CreaSE);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(168, 6);
            // 
            // eliminaToolStripMenuItem
            // 
            this.eliminaToolStripMenuItem.Name = "eliminaToolStripMenuItem";
            this.eliminaToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.eliminaToolStripMenuItem.Text = "Elimina";
            this.eliminaToolStripMenuItem.Click += new System.EventHandler(this.Elimina);
            // 
            // copiaToolStripMenuItem
            // 
            this.copiaToolStripMenuItem.Name = "copiaToolStripMenuItem";
            this.copiaToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.copiaToolStripMenuItem.Text = "Copia";
            // 
            // iNcollaToolStripMenuItem
            // 
            this.iNcollaToolStripMenuItem.Name = "iNcollaToolStripMenuItem";
            this.iNcollaToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.iNcollaToolStripMenuItem.Text = "Incolla";
            // 
            // cardImageList
            // 
            this.cardImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("cardImageList.ImageStream")));
            this.cardImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.cardImageList.Images.SetKeyName(0, "BSO.png");
            this.cardImageList.Images.SetKeyName(1, "DF.png");
            this.cardImageList.Images.SetKeyName(2, "EF.png");
            this.cardImageList.Images.SetKeyName(3, "SE.png");
            this.cardImageList.Images.SetKeyName(4, "rec.png");
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 5);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(357, 347);
            this.propertyGrid1.TabIndex = 0;
            // 
            // FileSystemUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "FileSystemUI";
            this.Size = new System.Drawing.Size(546, 352);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.FileSystemMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ImageList cardImageList;
        private System.Windows.Forms.ContextMenuStrip FileSystemMenu;
        private System.Windows.Forms.ToolStripMenuItem creaEFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creaEFTLVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creaDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creaBSOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creaSEToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem eliminaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copiaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iNcollaToolStripMenuItem;
    }
}
