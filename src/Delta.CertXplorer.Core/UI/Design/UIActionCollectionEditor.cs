/* 
 * Grabbed from Marco De Sanctis' Actions
 * see http://blogs.ugidotnet.org/crad/articles/38329.aspx
 * Original namespace: Crad.Windows.Forms.Actions
 * License: Common Public License Version 1.0
 * 
 */

using System;
using System.ComponentModel.Design;
using Delta.CertXplorer.UI.Actions;

namespace Delta.CertXplorer.UI.Design
{
    /// <summary>
    /// Design-Time Editor for <see cref="UIAction"/> objects collections.
    /// </summary>
    internal sealed class UIActionCollectionEditor : CollectionEditor
    {
        public UIActionCollectionEditor() : base(typeof(UIActionCollection)) { }

        protected override Type[] CreateNewItemTypes() => new Type[] { typeof(UIAction) };
    }
}
