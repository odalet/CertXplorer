using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VirtualSmartCard
{
    public partial class Main : Form
    {
        JWC.MruStripMenuInline mru;
        private int childFormNumber = 0;

        public Main()
        {
            InitializeComponent();
            mru = new JWC.MruStripMenuInline(fileMenu, toolStripMenuMRU, new JWC.MruStripMenu.ClickedHandler(mruClick), @"Software\VirtualSmartCard\MRU", true, 6);
            mru.LoadFromRegistry();
        }

        void mruClick(int num, string fileName)
        {
            VirtualCardForm childForm = new VirtualCardForm(this, fileName);
            childForm.Show();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            VirtualCardForm childForm = new VirtualCardForm(this);
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Binary Card|*.bsc";
            openFileDialog.DefaultExt = "bsc";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
                VirtualCardForm childForm = new VirtualCardForm(this,FileName);
                childForm.Show();
                mru.AddFile(FileName);
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild!=null)
                (ActiveMdiChild as VirtualCardForm).SalvaConNome(null, null);
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                (ActiveMdiChild as VirtualCardForm).SalvaFile(null, null);
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = false;
                NotifyIcon ni = new NotifyIcon();
                ni.Click += new EventHandler((s, ev) => {
                    ni.Dispose();
                    Visible = true;
                    WindowState = FormWindowState.Normal;
                });
                ni.Icon = Icon;
                ni.Visible = true;
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            mru.SaveToRegistry();
        }
    }
}
