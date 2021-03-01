using System.Windows.Forms;
using Delta.CertXplorer.ApplicationModel;

namespace Delta.CertXplorer.About
{
    internal sealed class AboutCertXplorerService : WindowsFormsAboutService
    {
        protected override Form CreateForm() => new AboutCertXplorerForm();
    }
}
