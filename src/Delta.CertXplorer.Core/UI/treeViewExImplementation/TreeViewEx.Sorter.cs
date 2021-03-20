using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    partial class TreeViewEx
    {
        protected class Sorter : IComparer, IComparer<TreeNode>
        {
            public Sorter() { }

            int IComparer.Compare(object x, object y) => x is TreeNode xn && y is TreeNode yn ? DoCompare(xn, yn) : 0;
            int IComparer<TreeNode>.Compare(TreeNode x, TreeNode y) => DoCompare(x, y);

            protected virtual int DoCompare(TreeNode x, TreeNode y) =>
                x is TreeNodeEx node1 && y is TreeNodeEx node2 ?
                node1.CompareTo(node2) :
                x.Text.CompareTo(y.Text);
        }
    }
}
