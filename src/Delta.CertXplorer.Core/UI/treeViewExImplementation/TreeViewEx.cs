using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    /// <summary>
    /// A TreeView allowing more control over the way it is represented and the way it behaves.
    /// </summary>
    public partial class TreeViewEx : TreeView
    {
        private ITreeNodeResolver expandedStateNodeResolver;
        private TreeViewExpandedStateSerializer treeViewExpandedStateSerializer;
        private ITreeViewExContextMenuStripProvider contextMenuStripProvider;
        private TreeNode mouseDownSelectedNode;
        private TreeNodeEx lastDragOverNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewEx"/> class.
        /// </summary>
        public TreeViewEx() : base()
        {
            ExpandedState = "";
            TreeViewNodeSorter = new Sorter();
            FullRowSelect = true;
            HideSelection = false;
            AllowDrop = true;

            treeViewExpandedStateSerializer = new TreeViewExpandedStateSerializer(this, BuiltinTreeNodeResolver.Text);
            expandedStateNodeResolver = treeViewExpandedStateSerializer.TreeNodeResolver;
            contextMenuStripProvider = new DefaultTreeViewExContextMenuStripProvider(this);
        }

        public event TreeNodeExEventHandler SelectedNodeChanged;
        public event TreeNodeExEventHandler NodeActivated;

        [DefaultValue(false)]
        public bool AllowDrag { get; set; }

        [DefaultValue(false)]
        public bool CanClearSelection { get; set; }

        [Browsable(false)]
        public IEnumerable<TreeNode> AllNodes => this.Find(null);

        [Browsable(false)]
        public bool IsEditing { get; private set; }

        [Browsable(false)]
        public bool IsCollapsing { get; private set; }

        [Browsable(false)]
        public bool IsExpanding { get; private set; }

        public ITreeViewExContextMenuStripProvider ContextMenuStripProvider
        {
            get => contextMenuStripProvider;
            set => contextMenuStripProvider = value ?? new DefaultTreeViewExContextMenuStripProvider(this);
        }

        private string ExpandedState { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ITreeNodeResolver ExpandedStateNodeResolver
        {
            get => expandedStateNodeResolver;
            set
            {
                treeViewExpandedStateSerializer = new TreeViewExpandedStateSerializer(this, value);
                expandedStateNodeResolver = treeViewExpandedStateSerializer.TreeNodeResolver;
            }
        }

        public void SaveExpandedState()
        {
            OnBeforeSaveExpandedState();
            ExpandedState = SerializeExpandedState();
            OnAfterSaveExpandedState();
        }

        public void RestoreExpandedState()
        {
            OnBeforeRestoreExpandedState();
            DeserializeExpandedState(ExpandedState);
            OnAfterRestoreExpandedState();
        }

        public void ActivateNode(TreeNodeEx node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.TreeView != this) throw new ArgumentException("This node doesn't belong to this treeview", nameof(node));

            OnBeforeActivateNode(node);

            var e = new CancelEventArgs(false);
            node.ActivateInternal(e);
            if (!e.Cancel) OnAfterActivateNode(node);
        }

        protected virtual void OnBeforeSaveExpandedState() { }
        protected virtual void OnAfterSaveExpandedState() { }
        protected virtual void OnBeforeRestoreExpandedState() { }
        protected virtual void OnAfterRestoreExpandedState() { }
        protected virtual void OnBeforeActivateNode(TreeNodeEx node) { }
        protected virtual void OnAfterActivateNode(TreeNodeEx node) => NodeActivated?.Invoke(this, new TreeNodeExEventArgs(node));

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F2:
                    BeginLabelEdit();
                    return true;

                case Keys.Return:
                case Keys.Space:
                    if (!IsEditing)
                    {
                        ActivateSelectedNode();
                        return true;
                    }
                    else break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (GetNodeAt(e.X, e.Y) is TreeNodeEx nodeEx)
            {
                mouseDownSelectedNode = nodeEx;
                mouseDownSelectedNode.ContextMenuStrip = ContextMenuStripProvider.GetContextMenuStrip(nodeEx);
            }
            else if (CanClearSelection) mouseDownSelectedNode = null;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (mouseDownSelectedNode != null)
            {
                if (e.Button == MouseButtons.Right) 
                    SelectedNode = mouseDownSelectedNode;
                else if (e.Button == MouseButtons.Left)
                {
                    if (SelectedNode == mouseDownSelectedNode)
                        BeginLabelEdit(mouseDownSelectedNode as TreeNodeEx, false);
                    else SelectedNode = mouseDownSelectedNode;
                }

                mouseDownSelectedNode = null;
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            if (e.Node is TreeNodeEx nodeEx && SelectedNodeChanged != null)
                SelectedNodeChanged(this, new TreeNodeExEventArgs(nodeEx));
        }

        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            IsCollapsing = true;
            mouseDownSelectedNode = null;

            if (e.Node is TreeNodeEx nodeEx) nodeEx.Collapsing(e);
        }

        protected override void OnAfterCollapse(TreeViewEventArgs e) => IsCollapsing = false;

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            IsExpanding = true;
            mouseDownSelectedNode = null;

            if (e.Node is TreeNodeEx nodeEx) nodeEx.Expanding(e);
        }

        protected override void OnAfterExpand(TreeViewEventArgs e) => IsExpanding = false;

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            SelectedNode = GetNodeAt(e.Location);
            ActivateSelectedNode();
        }        

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            IsEditing = true;

            e.Node.EnsureVisible();
            if (e.Node is TreeNodeEx node) node.OnBeforeLabelEdit();
            base.OnBeforeLabelEdit(e);
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            LabelEdit = false;

            e.CancelEdit = string.IsNullOrEmpty(e.Label);
            if (!e.CancelEdit)
            {
                if (e.Node is TreeNodeEx nodeEx)
                    nodeEx.OnAfterLabelEdit(e.Label);

                var nodes = e.Node.GetSiblings();
                SortNodes(nodes, false);

                // Refresh nodes: Needed to prevent display glitches
                var destination = new TreeNode[nodes.Count];
                nodes.CopyTo(destination, 0);
                nodes.Clear();
                nodes.AddRange(destination);
            }

            IsEditing = false;

            // We also need this, because we lose the currently selected node:
            // The selected node doesn't change, but root node is shown as selected
            SelectedNode = e.Node;

            base.OnAfterLabelEdit(e);
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);
            if (!AllowDrag) return;

            if (e.Item is TreeNodeEx node)
            {
                var data = node.GetDataObject();
                if (data != null)
                {
                    _ = DoDragDrop(data, DragDropEffects.All);
                    SortNodes(node.GetSiblings(), false);
                }
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            if (!AllowDrag) return;

            drgevent.Effect = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            if (!AllowDrag) return;

            var location = PointToClient(new Point(drgevent.X, drgevent.Y));
            if (GetNodeAt(location) is TreeNodeEx node)
            {
                const int controlKeyState = 8;
                static bool isControlKeyPressed(int keyState) => (keyState & controlKeyState) == controlKeyState;
                
                var effect = isControlKeyPressed(drgevent.KeyState) ? DragDropEffects.Copy : DragDropEffects.Move;
                drgevent.Effect = node.FilterEffect(effect);

                if (drgevent.Effect != DragDropEffects.None)
                {
                    if (lastDragOverNode != null && lastDragOverNode != node)
                        lastDragOverNode.DoDragLeave(drgevent);

                    node.DoDragOver(drgevent);
                    SelectedNode = lastDragOverNode = node;
                }
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            if (!AllowDrag) return;

            var location = PointToClient(new Point(drgevent.X, drgevent.Y));
            if (GetNodeAt(location) is TreeNodeEx node)
            {
                node.DoDragDrop(drgevent);
                SortNodes(node.GetSiblings(), false);
            }
        }

        private void ActivateSelectedNode()
        {
            if (SelectedNode is TreeNodeEx nodeEx) ActivateNode(nodeEx);
        }

        private string SerializeExpandedState() => treeViewExpandedStateSerializer.Serialize();

        private void DeserializeExpandedState(string state)
        {
            if (string.IsNullOrEmpty(state)) return;
            treeViewExpandedStateSerializer.Deserialize(state);
        }

        private void BeginLabelEdit() => BeginLabelEdit(SelectedNode as TreeNodeEx, true);
        private void BeginLabelEdit(TreeNodeEx node, bool forceEdit)
        {
            if (LabelEdit && node != null && node.LabelEdit)
            {
                if (SelectedNode != node) SelectedNode = node;
                LabelEdit = true;
                if (forceEdit) node.BeginEdit();
            }
        }

        private void SortNodes(TreeNodeCollection nodes, bool recursive)
        {
            var array = new TreeNode[nodes.Count];
            nodes.CopyTo(array, 0);
            Array.Sort(array, TreeViewNodeSorter);
            nodes.Clear();
            nodes.AddRange(array);

            if (recursive)
                foreach (var child in array) SortNodes(child.Nodes, true);
        }
    }
}