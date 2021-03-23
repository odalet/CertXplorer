using System;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    public delegate void TreeNodeExEventHandler(object sender, TreeNodeExEventArgs e);

    public sealed class TreeNodeExEventArgs : EventArgs
    {
        public TreeNodeExEventArgs(TreeNodeEx bstn) : base() => Node = bstn;
        public TreeNodeEx Node { get; }
    }

    public delegate void TreeNodeCollectionEventHandler(object sender, TreeNodeCollectionEventArgs e);

    public sealed class TreeNodeCollectionEventArgs : EventArgs
    {
        public TreeNodeCollectionEventArgs(TreeNodeCollection coll) : base() => Nodes = coll;
        public TreeNodeCollection Nodes { get; }
    }
}
