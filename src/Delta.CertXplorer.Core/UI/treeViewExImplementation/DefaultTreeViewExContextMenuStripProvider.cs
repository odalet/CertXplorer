using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    public class DefaultTreeViewExContextMenuStripProvider : ITreeViewExContextMenuStripProvider
    {
        protected TreeViewEx treeView = null;

        public DefaultTreeViewExContextMenuStripProvider(TreeViewEx treeView) => this.treeView = treeView;

        [SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "Example code")]
        public virtual ContextMenuStrip GetContextMenuStrip(TreeNodeEx node)
        {
            // Example
            //ContextMenuStrip strip = new ContextMenuStrip();
            //strip.Items.Add("Menu 1");
            //strip.Items.Add("Menu 2");
            //return strip;

            return null; 
        }
    }
}
