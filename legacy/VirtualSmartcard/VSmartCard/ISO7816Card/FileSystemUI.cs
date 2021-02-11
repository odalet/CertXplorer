using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ISO7816Card;
using ISO7816;
using ISO7816.FileSystem;
using VirtualSmartCard;

namespace CardModule
{
    public partial class FileSystemUI : UserControl
    {
        Font selectedFont;

        IISO7816Card card;
        public void SetCard(ICard ownedCard) {
            card = ownedCard as IISO7816Card;
            setOwner(this.card.MasterFile, this.card);
            card.objectChanged += new Action<ICard, ICardObject, ChangeType>(card_objectChanged);
            treeView1.Nodes.Clear();
            objNodes = new Dictionary<ICardObject, TreeNode>();

            card_objectChanged(card, card.MasterFile, ChangeType.Created);
            TreeAdd(card.MasterFile);
        }

        public FileSystemUI()
        {
            selectedFont = new Font(DefaultFont.FontFamily, DefaultFont.SizeInPoints, FontStyle.Bold, GraphicsUnit.Point);
            InitializeComponent();
            propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
        }

        void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var obj = propertyGrid1.SelectedObject as ICardWrapper;
            if (obj == null)
                return;
            if (!objNodes.ContainsKey(obj.getCardObject()))
                return;
            objNodes[obj.getCardObject()].Text = obj.getCardObject().ID.ToString("X04") + " " + obj.getCardObject().Description;
        }

        int getImageIndex(ICardObject obj)
        {
            if (obj is DF)
                return 1;
            else if (obj is EF)
                return 2;
            else if (obj is BSO)
                return 0;
            else if (obj is SecurityEnvironmenet)
                return 3;
            else return 0;
        }

        bool delNode(TreeNodeCollection coll, ICardObject obj)
        {
            foreach (TreeNode c in coll)
            {
                if (c.Tag == obj)
                {
                    c.Remove();
                    return true;
                }
                if (c.Nodes.Count > 0)
                    if (delNode(c.Nodes, obj))
                        return true;
            }
            return false;
        }

        class RecObj : IObjectWithData
        {
            IObjectWithDataRecords parent;
            int numRec;
            public RecObj(IObjectWithDataRecords parent, int numRec)
            {
                this.parent = parent;
                this.numRec = numRec;
            }

            public byte[] Data
            {
                get
                {
                    return parent.Data[numRec];
                }
                set
                {
                    parent.Data[numRec] = value;
                }
            }
        }
        Dictionary<ICardObject, TreeNode> objNodes = new Dictionary<ICardObject, TreeNode>();
        void card_objectChanged(ICard card, ICardObject obj, ChangeType change)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Card, ICardObject, ChangeType>((a, b, c) =>
                {
                    card_objectChanged(a, b, c);
                }), card, obj, change);
                return;
            }
            if (change == ChangeType.Deleted)
            {
                delNode(treeView1.Nodes, obj);
            }

            if (change == ChangeType.Created)
            {
                TreeNodeCollection parentNode = null;
                if (obj.Parent == null)
                    parentNode = treeView1.Nodes;
                else
                    parentNode = objNodes[obj.Parent].Nodes;

                TreeNode node = parentNode.Add(obj.ID.ToString("X04") + " " + obj.Description);
                node.ImageIndex = getImageIndex(obj);
                node.SelectedImageIndex = node.ImageIndex;
                node.Tag = obj;
                objNodes[obj] = node;
                if (obj is IObjectWithDataRecords)
                {
                    AddRecords(obj, node);
                }
                return;
            }
            else if (change == ChangeType.Selected)
            {
                objNodes[obj].NodeFont = selectedFont;
                return;
            }
            else if (change == ChangeType.Unselected)
            {
                objNodes[obj].NodeFont = DefaultFont;
                return;
            }
            else if (change == ChangeType.Modified)
            {
                if (obj is IObjectWithDataRecords)
                {
                    objNodes[obj].Nodes.Clear();
                    AddRecords(obj, objNodes[obj]);
                }
            }
        }

        private static void AddRecords(ICardObject obj, TreeNode node)
        {
            var recs = obj as IObjectWithDataRecords;
            int numRec = 0;
            if (recs.Data == null)
                return;
            foreach (var v in recs.Data)
            {
                var recNode = node.Nodes.Add("Record " + numRec.ToString());
                recNode.Tag = new RecObj(recs, numRec);
                recNode.ImageIndex = recNode.SelectedImageIndex = 4;
                numRec++;
            }
        }
        WrapperMF WrapperMF = new WrapperMF();
        WrapperDF WrapperDF = new WrapperDF();
        WrapperEF WrapperEFBin = new WrapperEFBinary();
        WrapperEF WrapperEFRec = new WrapperEFRecord();
        WrapperBSO WrapperBSO = new WrapperBSO();
        WrapperSE WrapperSE = new WrapperSE();

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Tag is MF)
            {
                WrapperMF.setObject(treeView1.SelectedNode.Tag as MF);
                propertyGrid1.SelectedObject = WrapperMF;
            }
            else if (treeView1.SelectedNode.Tag is DF)
            {
                WrapperDF.setObject(treeView1.SelectedNode.Tag as DF);
                propertyGrid1.SelectedObject = WrapperDF;
            }
            else if (treeView1.SelectedNode.Tag is EFBinary)
            {
                WrapperEFBin.setObject(treeView1.SelectedNode.Tag as EF);
                propertyGrid1.SelectedObject = WrapperEFBin;
            }
            else if (treeView1.SelectedNode.Tag is EFRecord)
            {
                WrapperEFRec.setObject(treeView1.SelectedNode.Tag as EF);
                propertyGrid1.SelectedObject = WrapperEFRec;
            }
            else if (treeView1.SelectedNode.Tag is BSO)
            {
                WrapperBSO.setObject(treeView1.SelectedNode.Tag as BSO);
                propertyGrid1.SelectedObject = WrapperBSO;
            }
            else if (treeView1.SelectedNode.Tag is SecurityEnvironmenet)
            {
                WrapperSE.setObject(treeView1.SelectedNode.Tag as SecurityEnvironmenet);
                propertyGrid1.SelectedObject = WrapperSE;
            }

        }
        private void CreaEF(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            ICardObject obj = treeView1.SelectedNode.Tag as ICardObject;
            if (!(obj is DF))
                return;
            DF df = obj as DF;
            card.CreateEF(1, df, 10);
        }

        private void CreaDF(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            ICardObject obj = treeView1.SelectedNode.Tag as ICardObject;
            if (!(obj is DF))
                return;
            DF df = obj as DF;
            card.CreateDF(1, df);
        }

        private void CreaBSO(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            ICardObject obj = treeView1.SelectedNode.Tag as ICardObject;
            if (!(obj is DF))
                return;
            DF df = obj as DF;
            card.CreateBSO(1, df);
        }

        void TreeAdd(DF df)
        {
            foreach (var v in df.Childs)
            {
                card_objectChanged(v.Owner, v, ChangeType.Created);
                if (v is DF)
                    TreeAdd(v as DF);
            }
        }
        void setOwner(ICardObject obj, IISO7816Card owner)
        {
            obj.Owner = owner;
            if (obj is DF)
            {
                DF df = obj as DF;
                foreach (var c in df.Childs)
                {
                    setOwner(c, owner);
                }
            }
        }

        private void CreaSE(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            ICardObject obj = treeView1.SelectedNode.Tag as ICardObject;
            if (!(obj is DF))
                return;
            DF df = obj as DF;
            card.CreateSE(1, df);
        }

        private void Elimina(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            ICardObject obj = treeView1.SelectedNode.Tag as ICardObject;
            if (obj.Parent!=null)
                obj.Parent.RemoveChild(obj);
        }
        private void creaEFTLVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            ICardObject obj = treeView1.SelectedNode.Tag as ICardObject;
            if (!(obj is DF))
                return;
            DF df = obj as DF;
            card.CreateEFLinearTLV(1, df, 10);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;
            if (treeView1.SelectedNode.Tag is IObjectWithData)
            {
                var obj = treeView1.SelectedNode.Tag as IObjectWithData;
                var blobform = new BlobForm(obj.Data);
				if (blobform.ShowDialog() == DialogResult.OK)
				{
					obj.Data = blobform.Data;
					treeView1_AfterSelect(null, new TreeViewEventArgs(treeView1.SelectedNode));

				}
            }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) {
                Elimina(sender, EventArgs.Empty);
            }
        }
    }
}
