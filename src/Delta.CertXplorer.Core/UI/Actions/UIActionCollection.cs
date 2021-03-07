/* 
 * Grabbed from Marco De Sanctis' Actions
 * see http://blogs.ugidotnet.org/crad/articles/38329.aspx
 * Original namespace: Crad.Windows.Forms.Actions
 * License: Common Public License Version 1.0
 * 
 */

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using Delta.CertXplorer.UI.Design;

namespace Delta.CertXplorer.UI.Actions
{
    [Editor(typeof(UIActionCollectionEditor), typeof(UITypeEditor))]
    public class UIActionCollection : Collection<UIAction>
    {
        public UIActionCollection(UIActionsManager parentList) => Parent = parentList;

        public UIActionsManager Parent { get; }

        protected override void ClearItems()
        {
            foreach (var action in this) action.ActionList = null;
            base.ClearItems();
        }

        protected override void InsertItem(int index, UIAction item)
        {
            // This check is needed because Delta.CertXplorer.ApplicationModel.BaseDockingForm may add the
            // same item multiple times...
            if (Contains(item)) return;

            base.InsertItem(index, item);
            item.ActionList = Parent;
        }

        protected override void RemoveItem(int index)
        {
            this[index].ActionList = null;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, UIAction item)
        {
            if (Count > index) this[index].ActionList = null;
            base.SetItem(index, item);

            item.ActionList = Parent;
        }
    }
}
