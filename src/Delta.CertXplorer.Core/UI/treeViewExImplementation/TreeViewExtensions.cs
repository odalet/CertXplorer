using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    public static class TreeViewExtensions
    {
        public static TreeNodeCollection GetSiblings(this TreeNode node) => node.Parent == null ? node.TreeView.Nodes : node.Parent.Nodes;

        public static IEnumerable<TreeNode> Find(this TreeView treeView, Func<TreeNode, bool> predicate) => Find(treeView.Nodes, predicate);
        public static IEnumerable<TreeNode> Find(this TreeNodeCollection treeNodes, Func<TreeNode, bool> predicate) => Find(treeNodes.Cast<TreeNode>(), predicate);
        public static IEnumerable<TreeNode> Find(this TreeNode treeNode, Func<TreeNode, bool> predicate) => Find(new TreeNode[] { treeNode }, predicate);
        private static IEnumerable<TreeNode> Find(IEnumerable<TreeNode> nodes, Func<TreeNode, bool> predicate)
        {
            if (nodes == null) return new TreeNode[0];

            var result = new List<TreeNode>();
            if (predicate == null) result.AddRange(nodes);
            else result.AddRange(nodes.Where(predicate));

            // children
            foreach (var node in nodes)
                result.AddRange(Find(node.Nodes, predicate));

            return result;
        }
    }
}
