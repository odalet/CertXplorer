using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Delta.CertXplorer.UI;

namespace Delta.CertXplorer.ApplicationModel
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms conventions")]
    public partial class WindowsFormsAboutServiceForm : Form
    {
        private readonly ListViewGroup certXplorerGroup;
        private readonly ListViewGroup othersGroup;
        private readonly WindowsFormsAboutService aboutService;

        public WindowsFormsAboutServiceForm() : this(This.GetService<IAboutService>() as WindowsFormsAboutService) { }
        public WindowsFormsAboutServiceForm(WindowsFormsAboutService service)
        {
            InitializeComponent();

            certXplorerGroup = new("Delta.CertXplorer", HorizontalAlignment.Left);
            othersGroup = new(SR.Others, HorizontalAlignment.Left);

            if (service == null) service = new WindowsFormsAboutService();
            aboutService = service;

            Text = string.Format(SR.AboutNameVersion, AssemblyTitle, AssemblyVersion);
            labelApplication.Text = AssemblyTitle;
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = string.Format(SR.VersionWithValue, AssemblyVersion);
            if (IsDebugBuild)
                labelVersion.Text += " (DEBUG Build)";
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = AssemblyDescription;
        }

        public string AssemblyTitle => aboutService.AssemblyTitle;
        public string AssemblyVersion => aboutService.AssemblyVersion;
        public string AssemblyDescription => aboutService.AssemblyDescription;
        public string AssemblyProduct => aboutService.AssemblyProduct;
        public string AssemblyCopyright => aboutService.AssemblyCopyright;
        public string AssemblyCompany => aboutService.AssemblyCompany;

        public bool IsDebugBuild =>
#if DEBUG
                true;
#else
                false;
#endif


        public Image Splash
        {
            get => logoPictureBox.Image;
            set => logoPictureBox.Image = value;
        }

        public string Credits => aboutService.Credits;
        public string History => aboutService.History;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (string.IsNullOrEmpty(Credits))
                Tabs.TabPages.Remove(tpCredits);
            else tbCredits.Text = Credits;

            if (string.IsNullOrEmpty(History))
                Tabs.TabPages.Remove(tpHistory);
            else tbHistory.Text = History;

            try
            {
                FillAssemblyList();
            }
            catch (Exception ex)
            {
                This.Logger.Error(ex); 
            }
        }

        protected virtual ListViewGroup[] CreateGroups() => new ListViewGroup[] { certXplorerGroup };
        protected virtual ListViewGroup GetGroup(ListViewItem item) => item.Text.ToLowerInvariant().StartsWith("Delta.CertXplorer") ? certXplorerGroup : null;

        private void FillAssemblyList()
        {
            var groups = CreateGroups();
            // We don't sort the group names: they must be provided in the expected order.
            lvModules.Groups.AddRange(groups);
            _ = lvModules.Groups.Add(othersGroup); // Others is always the last group.

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(ass => !ass.IsDynamic());

            var listViewItems = new List<ListViewItem>();
            foreach (var assembly in assemblies)
            {
                var listViewItem = CreateItem(assembly);
                if (listViewItem != null) listViewItems.Add(listViewItem);
            }

            foreach (var item in listViewItems.OrderBy(item =>
                Path.GetFileNameWithoutExtension(item.Text)))
            {
                item.Group = GetGroup(item);
                if (item.Group == null) item.Group = othersGroup;
                _ = lvModules.Items.Add(item);
            }
        }

        private ListViewItem CreateItem(Assembly assembly)
        {
            try
            {
                var title = assembly.GetTitle(true);
                var version = assembly.GetName().Version.ToString();
                var path = assembly.GetLocation();

                var item = new ListViewItem
                {
                    Text = title,
                    Tag = assembly,
                    ToolTipText = $"{title} {version}\r\n{assembly.GetDescription()}"
                };

                _ = item.SubItems.Add(version);
                _ = item.SubItems.Add(path);

                return item;
            }
            catch (Exception ex)
            {
                This.Logger.Error(ex);
                return null;
            }
        }

        private void pcsLinks_SaveClick(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "All Files|*.*|Text Files (*.txt)|*.txt",
                FilterIndex = 0,
                DefaultExt = "*.*",
                OverwritePrompt = true,
                SupportMultiDottedExtensions = true,
                CheckPathExists = true,
                ValidateNames = true
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                var filename = string.Empty;
                try
                {
                    filename = dialog.FileName;
                    using var writer = File.CreateText(filename);
                    writer.Write(aboutService.GetAboutText());
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    This.Logger.Error($"Error while saving data into file file://{filename}: {ex.Message}", ex);
                    _ = ErrorBox.Show(this, $"An error occurred while saving a file: {ex.Message}");
                }
            }
        }

        private void pcsLinks_CopyClick(object sender, EventArgs e)
        {
            try { Clipboard.SetText(aboutService.GetAboutText()); }
            catch (Exception ex)
            {
                This.Logger.Error(string.Format("Error while copying data to the clipboard: {0}",
                    ex.Message), ex);
            }
        }
    }
}
