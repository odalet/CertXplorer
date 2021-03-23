using System;
using System.Windows.Forms;
using System.Globalization;

namespace Delta.CertXplorer.UI
{
    public sealed class TreeViewExpandedStateSerializer
    {
        private sealed class TextTreeNodeResolver : ITreeNodeResolver
        {
            public TreeNode FindNode(TreeNodeCollection nodes, string key)
            {
                foreach (TreeNode tn in nodes) 
                {
                    if (tn.Text == key) 
                        return tn; 
                }

                return null;
            }

            public string GetNodeKey(TreeNode node) => node.Text;
        }

        private sealed class NameTreeNodeResolver : ITreeNodeResolver
        {
            public TreeNode FindNode(TreeNodeCollection nodes, string key) => nodes.ContainsKey(key) ? nodes[key] : null;

            public string GetNodeKey(TreeNode node) => node.Name;
        }

        private readonly TreeView treeview;
        private readonly ITreeNodeResolver treeNodeResolver;

        public TreeViewExpandedStateSerializer(TreeView tv) : this(tv, BuiltinTreeNodeResolver.Name) { }
        public TreeViewExpandedStateSerializer(TreeView tv, BuiltinTreeNodeResolver resolver) : this(tv, GetResolver(resolver)) { }
        public TreeViewExpandedStateSerializer(TreeView tv, ITreeNodeResolver resolver)
        {
            treeview = tv ?? throw new ArgumentNullException(nameof(tv));
            treeNodeResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public ITreeNodeResolver TreeNodeResolver => treeNodeResolver;

        public string Serialize()
        {
            var innerState = string.Empty;
            if (treeview.Nodes != null && treeview.Nodes.Count > 0)
                innerState = SerializeCollection(treeview.Nodes);
            return "{" + innerState + "}";
        }

        public void Deserialize(string state)
        {
            treeview.CollapseAll();
            DeserializeCollection(treeview.Nodes, state);
        }

        private static ITreeNodeResolver GetResolver(BuiltinTreeNodeResolver builtin) => builtin switch
        {
            BuiltinTreeNodeResolver.Text => new TextTreeNodeResolver(),
            BuiltinTreeNodeResolver.Name => new NameTreeNodeResolver(),
            _ => null,
        };

        private string SerializeCollection(TreeNodeCollection collection)
        {
            var state = string.Empty;
            foreach (TreeNode tn in collection)
            {
                if (tn.Nodes != null && tn.Nodes.Count > 0)
                {
                    var status = ":" + tn.IsExpanded.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
                    var innerState = string.Empty;
                    innerState = SerializeCollection(tn.Nodes);
                    state += "{" + SerializeText(treeNodeResolver.GetNodeKey(tn)) + status + innerState + "}";
                }
            }

            return state;
        }

        private void DeserializeCollection(TreeNodeCollection collection, string state)
        {
            if (string.IsNullOrEmpty(state)) return;

            while (state[0] == '{' && state[state.Length - 1] == '}')
            {
                state = state.Substring(1, state.Length - 2);
                state = state.Trim();
            }

            if (string.IsNullOrEmpty(state)) return;

            var index = 0;
            var currentNodeText = string.Empty;
            TreeNode lastNode;
            while (true)
            {
                if (index >= state.Length) 
                    break;
                
                if (state[index] == '{')
                {
                    lastNode = TryExpand(collection, currentNodeText.Trim());
                    var next = FindClosingBrace(state.Substring(index)) + index;
                    if (next <= index) // on tente quand même...
                    {
                        if (lastNode != null)
                            DeserializeCollection(lastNode.Nodes, state.Substring(index));
                        break;
                    }
                    else
                    {
                        if (lastNode != null)
                            DeserializeCollection(lastNode.Nodes, state.Substring(index, next - index + 1));
                        index = next + 1;
                    }
                }
                else
                {
                    currentNodeText += state[index];
                    index++;
                }
            }

            _ = TryExpand(collection, currentNodeText.Trim());
        }

        private TreeNode TryExpand(TreeNodeCollection nodes, string text)
        {
            var trueString = true.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            var expand = true;
            var nodeText = text;
            var index = text.IndexOf(':');
            if (index != -1)
            {
                nodeText = DeserializeText(text.Substring(0, index));
                expand = string.Compare(text.Substring(index + 1), trueString, true) == 0;
            }

            var tn = treeNodeResolver.FindNode(nodes, nodeText);
            if (tn != null && expand) tn.Expand();
            return tn;
        }

        private int FindClosingBrace(string text)
        {
            var count = 0;
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '{') count++;
                if (text[i] == '}')
                {
                    count--;
                    if (count == 0) return i;
                }
            }

            return -1;
        }

        private string SerializeText(string text) => text
            .Replace("\\", "\\5C")
            .Replace(":", "\\3A")
            .Replace("{", "\\7B")
            .Replace("}", "\\7D")
            ;

        private string DeserializeText(string text) => text
            .Replace("\\7D", "}")
            .Replace("\\7B", "{")
            .Replace("\\3A", ":")
            .Replace("\\5C", "\\")
            ;
    }
}
