using System.Windows.Forms;

namespace Delta.CertXplorer.UI.ToolWindows
{
    public partial class PropertyWindowControl : UserControl
    {
        public PropertyWindowControl() => InitializeComponent();

        public object SelectedObject
        {
            get => pg.SelectedObject;
            set => pg.SelectedObject = value;
        }
    }
}
