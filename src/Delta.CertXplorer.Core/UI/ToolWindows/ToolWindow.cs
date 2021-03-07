using System;
using Delta.CertXplorer.ApplicationModel;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer.UI.ToolWindows
{
    public partial class ToolWindow : DockContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow"/> class.
        /// </summary>
        public ToolWindow()
        {
            InitializeComponent();

            DockAreas = DockAreas.Float |
                DockAreas.DockBottom | DockAreas.DockTop |
                DockAreas.DockLeft | DockAreas.DockRight;

            var service = This.GetService<ILayoutService>();
            if (service != null) service.RegisterForm(Guid.ToString("D"), this);

            DockHandler.GetPersistStringCallback = () => Guid.ToString();
        }

        public virtual Guid Guid => Guid.Empty;
        protected virtual DockState DefaultDockState => DockState.DockLeft;

        public void DockDefault() => DockState = DefaultDockState;
    }
}
