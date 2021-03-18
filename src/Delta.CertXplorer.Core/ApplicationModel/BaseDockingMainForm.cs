using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Delta.CertXplorer.UI.Actions;
using Delta.CertXplorer.UI.Design;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer.ApplicationModel
{
    public partial class BaseDockingMainForm : Form, IExtenderProvider
    {
        private bool toolstripPanelsFixed = false;
        private ToolStripRenderMode renderMode = ToolStripRenderMode.ManagerRenderMode;

        public BaseDockingMainForm() => InitializeComponent();

        public ToolStripRenderMode RenderMode
        {
            get => renderMode;
            set
            {
                if (renderMode == value) return;
                renderMode = value;
                menuStrip.RenderMode = renderMode;
                topToolStripPanel.RenderMode = renderMode;
                bottomToolStripPanel.RenderMode = renderMode;
                leftToolStripPanel.RenderMode = renderMode;
                rightToolStripPanel.RenderMode = renderMode;
            }
        }

        [Editor(typeof(UIActionCollectionEditor), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UIActionCollection Actions => actionsManager.Actions;

        public ToolStripPanel TopToolStripPanel => topToolStripPanel;
        public ToolStripPanel LeftToolStripPanel => leftToolStripPanel;
        public ToolStripPanel RightToolStripPanel => rightToolStripPanel;
        public ToolStripPanel BottomToolStripPanel => bottomToolStripPanel;

        protected DockPanel Workspace => workspace;

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (toolstripPanelsFixed) return;

            // This fixes the appearance of the vertical toolstrip panels:
            // If we don't do run the code below, the toolstrip panels may appear
            // layed out like the figure on the left, and we want them like 
            // the figure on the right:
            //
            // |-------------|            ---------------
            // |             |            |             |
            // |             |            |             |
            // |             |            |             |
            // ---------------            ---------------
            // 
            // To achieve this, we reorder the child index so that the vertical panels
            // are created first. This ensures that the latter (the horizontal panels) 
            // will expand to their full width.

            Controls.SetChildIndex(rightToolStripPanel, 0);
            Controls.SetChildIndex(leftToolStripPanel, 1);
            Controls.SetChildIndex(topToolStripPanel, 2);
            Controls.SetChildIndex(bottomToolStripPanel, 3);
            toolstripPanelsFixed = true;
        }

        [DefaultValue(null)]
        public UIAction GetAction(Component extendee) => actionsManager.GetAction(extendee);

        public void SetAction(Component extendee, UIAction action)
        {
            // There may be a problem: an action can be set to an extendee before it is part 
            // of the manager actions list...
            // So we add it here to the collection (if it doesn't exist)
            // This means each action will probably be added twice to the collection.
            // This is not really an issue as the UIActionCollection checks for duplicates 
            // before adding actions.
            if (!actionsManager.Actions.Contains(action)) actionsManager.Actions.Add(action);

            actionsManager.SetAction(extendee, action);
        }

        bool IExtenderProvider.CanExtend(object extendee) => ((IExtenderProvider)actionsManager).CanExtend(extendee);
    }
}
