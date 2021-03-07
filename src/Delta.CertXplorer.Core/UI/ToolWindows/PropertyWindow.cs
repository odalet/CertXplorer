using System;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer.UI.ToolWindows
{
    public partial class PropertyWindow : ToolWindow
    {
        public PropertyWindow()
        {
            InitializeComponent();

            Icon = Properties.Resources.PropertiesIcon;
            TabText = SR.Properties;
            Text = SR.Properties;
            ToolTipText = SR.PropertiesWindow;
        }

        public override Guid Guid => new Guid("{1797B309-958E-4fcd-875F-6FD251FDD811}");

        public object SelectedObject
        {
            get => propertyControl.SelectedObject;
            set => propertyControl.SelectedObject = value;
        }

        protected override DockState DefaultDockState => DockState.DockRight;
    }
}
