using System.ComponentModel;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    public class FolderTreeNode : TreeNodeEx
    {
        private int collapsedImageIndex = -1;
        private int selectedCollapsedImageIndex = -1;

        public FolderTreeNode() : base() { }
        public FolderTreeNode(string text) : base(text) { }

        [DefaultValue(-1)]
        public int CollapsedImageIndex
        {
            get => collapsedImageIndex;
            set
            {
                collapsedImageIndex = value;
                if (ImageIndex == -1) ImageIndex = value;
            }
        }

        [DefaultValue(-1)]
        public int SelectedCollapsedImageIndex
        {
            get => selectedCollapsedImageIndex;
            set
            {
                selectedCollapsedImageIndex = value;
                if (SelectedImageIndex == -1) SelectedImageIndex = value;
            }
        }

        [DefaultValue(-1)]
        public int ExpandedImageIndex { get; set; }

        [DefaultValue(-1)]
        public int SelectedExpandedImageIndex { get; set; }

        protected override void Expanding(TreeViewCancelEventArgs e)
        {
            base.Expanding(e);
            SetExpandedState(true);
        }

        protected override void Collapsing(TreeViewCancelEventArgs e)
        {
            base.Collapsing(e);
            SetExpandedState(false);
        }

        private void SetExpandedState(bool expanded)
        {
            if (expanded)
            {
                ImageIndex = ExpandedImageIndex;
                SelectedImageIndex = SelectedExpandedImageIndex;
            }
            else 
            {
                ImageIndex = CollapsedImageIndex;
                SelectedImageIndex = SelectedCollapsedImageIndex;
            }
        }
    }
}
