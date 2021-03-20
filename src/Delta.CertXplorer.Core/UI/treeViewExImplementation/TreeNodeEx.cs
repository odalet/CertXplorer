using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace Delta.CertXplorer.UI
{
    /// <summary>
    /// Reprents a specialized Tree node (to be used with <see cref="TreeViewEx"/>).
    /// </summary>
    public class TreeNodeEx : TreeNode, IComparable<TreeNodeEx>, IEditActionsHandler
    {
        private DateTime dragOverTimer = DateTime.MaxValue;
        private EditActionState editActionState = EditActionState.None;

        public TreeNodeEx() : this(null) { }
        public TreeNodeEx(string text) : base(text)
        {
            DelayedDragOverAction = true;
            DragOverActionDelay = new(0, 0, 1);
            LabelEdit = true;
        }

        public bool DelayedDragOverAction { get; set; }
        public TimeSpan DragOverActionDelay { get; set; }
        public bool LabelEdit { get; set; }

        public TreeViewEx TreeViewEx => TreeView as TreeViewEx;
        public IEnumerable<TreeNode> AllNodes => this.Find(null);

        public EditActionState EditActionState
        {
            get => editActionState;
            set
            {
                if (value == editActionState) return;

                editActionState = value;
                OnEditActionStateChanged(editActionState);

                foreach (TreeNode tn in Nodes)
                {
                    if (tn is TreeNodeEx nodeEx) nodeEx.EditActionState = value;
                }
            }
        }

        public virtual bool CanCut => false;
        public virtual bool CanCopy => false;
        public virtual bool CanPaste => false;
        public virtual bool CanDelete => false;
        public virtual bool CanSelectAll => false;

        protected virtual int NodeOrder => 0;

        private bool IsDragOverTimerReset => dragOverTimer == DateTime.MaxValue;
        private bool IsDragOverTimeElapsed => DateTime.Now - dragOverTimer >= DragOverActionDelay;

        public void Activate() => TreeViewEx.ActivateNode(this);

        public virtual int CompareTo(TreeNodeEx other) =>
            NodeOrder != other.NodeOrder ?
            NodeOrder - other.NodeOrder :
            Text.CompareTo(other.Text);

        public virtual void Cut() { }
        public virtual void Copy() { }
        public virtual void Paste() { }
        public virtual void Delete() { }
        public virtual void SelectAll() { }

        protected internal virtual void OnBeforeLabelEdit() { }
        protected internal virtual void OnAfterLabelEdit(string newLabel) => Text = newLabel;
        protected internal virtual void Expanding(TreeViewCancelEventArgs e) { }
        protected internal virtual void Collapsing(TreeViewCancelEventArgs e) { }
        protected virtual void OnActivate(CancelEventArgs e) { }
        protected internal virtual void DoDragDrop(DragEventArgs drgevent) => OnDragDrop(drgevent);
        protected virtual void OnEditActionStateChanged(EditActionState state) { }

#pragma warning disable IDE0022 // Use expression body for methods
        protected internal virtual DataObject GetDataObject() =>
#if DEBUG
            new(ToString());
#else
            null;
#endif


        protected internal virtual DragDropEffects FilterEffect(DragDropEffects defaultEffect) =>
#if DEBUG
            defaultEffect;
#else
            DragDropEffects.None;
#endif

        protected virtual void OnDragOver(DragEventArgs drgevent)
        {
#if DEBUG

            Expand();
#endif
        }

        protected virtual void OnDragDrop(DragEventArgs drgevent)
        {
#if DEBUG
            Expand();
#endif
        }
#pragma warning restore IDE0022 // Use expression body for methods

        internal void ActivateInternal(CancelEventArgs e) => OnActivate(e);

        internal void DoDragOver(DragEventArgs drgevent)
        {
            if (DelayedDragOverAction)
            {
                if (IsDragOverTimerReset) InitializeDragOverTimer();
                else
                {
                    if (IsDragOverTimeElapsed)
                    {
                        ResetDragOverTimer();
                        OnDragOver(drgevent);
                    }
                }
            }
            else OnDragOver(drgevent);
        }

        internal void DoDragLeave(DragEventArgs _) => ResetDragOverTimer();
        
        private void ResetDragOverTimer() => dragOverTimer = DateTime.MaxValue;
        private void InitializeDragOverTimer() => dragOverTimer = DateTime.Now;
    }
}
