using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Delta.CertXplorer.UI.ToolWindows;
using WeifenLuo.WinFormsUI.Docking;

namespace Delta.CertXplorer.ApplicationModel
{
    /// <summary>
    /// Default implementation of <see cref="ILayoutService"/>.
    /// </summary>
    public sealed class LayoutService : ILayoutService
    {
        private sealed class EventsLock : IDisposable
        {
            private readonly LayoutService parent;

            public EventsLock(LayoutService layoutService)
            {
                parent = layoutService;
                parent.EventsLocked = true;
            }

            public void Dispose() => parent.EventsLocked = false;
        }

        private sealed class FormLayout
        {
            public FormLayout() => DockingInfo = string.Empty;

            public Rectangle? Bounds { get; set; }
            public FormWindowState? WindowState { get; set; }
            public string DockingInfo { get; set; }
            public string AdditionalData { get; set; }
        }

        private readonly string layoutSettingsFileName;
        private readonly List<string> loadedForms = new();
        private readonly Dictionary<string, Form> formsByKey = new();
        private readonly Dictionary<string, FormLayout> layouts = new();
        private readonly Dictionary<string, DockPanel> workspaces = new();
        private readonly Dictionary<string, Dictionary<Guid, ToolWindow>> toolWindows = new();
        private Rectangle defaultBounds = Rectangle.Empty;

        public LayoutService(string fileName)
        {
            layoutSettingsFileName = fileName;
            if (File.Exists(layoutSettingsFileName)) ReadLayouts();
        }

        private Rectangle DefaultBounds
        {
            get
            {
                if (defaultBounds.IsEmpty)
                {
                    var screen = Screen.PrimaryScreen.WorkingArea;
                    var wmargin = screen.Width / 10;
                    var hmargin = screen.Height / 10;

                    defaultBounds = new Rectangle(
                        screen.Left + wmargin,
                        screen.Top + hmargin,
                        screen.Width - 2 * wmargin,
                        screen.Height - 2 * hmargin);
                }

                return defaultBounds;
            }
        }

        private bool EventsLocked { get; set; }

        public void RegisterForm(string key, Form form, DockPanel workspace)
        {
            if (formsByKey.ContainsKey(key))
                _ = formsByKey.Remove(key);
            formsByKey.Add(key, form);
            LoadForm(key);
            form.Load += (s, e) => LoadForm(key);

            if (workspace != null)
            {
                if (workspaces.ContainsKey(key))
                    _ = workspaces.Remove(key);
                workspaces.Add(key, workspace);
            }
        }

        public void RegisterToolWindow(string key, ToolWindow window)
        {
            if (window == null) throw new ArgumentNullException("window");
            if (!workspaces.ContainsKey(key)) throw new InvalidOperationException(
                "You can't register a tool window, if a workspace was not registered first.");

            Dictionary<Guid, ToolWindow> subDictionary;
            if (toolWindows.ContainsKey(key))
            {
                subDictionary = toolWindows[key];
                if (subDictionary.ContainsKey(window.Guid)) throw new InvalidOperationException(
                    $"Tool window with Guid {window.Guid} was already registered.");
            }
            else
            {
                subDictionary = new Dictionary<Guid, ToolWindow>();
                toolWindows.Add(key, subDictionary);
            }

            subDictionary.Add(window.Guid, window);
        }

        public bool RestoreDockingState(string key)
        {
            if (!layouts.ContainsKey(key)) return false;
            if (!workspaces.ContainsKey(key)) return false;

            var dockingXml = layouts[key].DockingInfo;
            try
            {
                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream);

                writer.Write(dockingXml);
                writer.Flush();
                _ = stream.Seek(0L, SeekOrigin.Begin);
                workspaces[key].LoadFromXml(stream, target =>
                {
                    var log = This.Logger;

                    if (string.IsNullOrEmpty(target))
                    {
                        log.Info("Unable to load window: no guid was provided");
                        return null;
                    }

                    var guid = Guid.Empty;
                    try
                    {
                        guid = new Guid(target);
                    }
                    catch (Exception ex)
                    {
                        var debugException = ex;
                    }

                    if (guid == Guid.Empty)
                    {
                        log.Info($"Unable to load window with guid {target}.");
                        return null;
                    }

                    ToolWindow window = null;
                    if (toolWindows.ContainsKey(key))
                    {
                        var dict = toolWindows[key];
                        if (dict.ContainsKey(guid)) window = dict[guid];
                    }

                    if (window == null)
                    {
                        log.Info($"Unable to load window with guid {target}.");
                        return null;
                    }

                    return window;

                }, false);

                writer.Close();
            }
            catch (Exception ex)
            {
                This.Logger.Error($"Unable to restore docking state from the layout key {key}.", ex);
                return false;
            }

            return true;
        }

        private IDisposable LockEvents() => new EventsLock(this);

        private void ReadLayouts()
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(layoutSettingsFileName);
                foreach (XmlNode xn in doc.ChildNodes)
                {
                    if (xn.Name != "layouts") continue;

                    foreach (XmlNode xnRoot in xn.ChildNodes)
                    {
                        if (xnRoot.Name != "layout") continue;

                        var key = GetNodeValue(xnRoot, "key");
                        if (key == null) continue;

                        try
                        {
                            ReadLayout(xnRoot, key);
                        }
                        catch (Exception ex)
                        {
                            This.Logger.Error(
                                $"Unable to deserialize layout information from file {layoutSettingsFileName} for key {key}.", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                This.Logger.Error(
                    $"Unable to deserialize layout information from file {layoutSettingsFileName}.", ex);
            }
        }

        private void ReadLayout(XmlNode xnRoot, string key)
        {
            if (layouts.ContainsKey(key)) return;

            var formLayout = new FormLayout();
            foreach (XmlNode xn in xnRoot.ChildNodes)
            {
                if (xn.Name == "bounds")
                {
                    var bounds = GetNodeValue(xn, "value");
                    if (bounds != null)
                    {
                        try
                        {
                            formLayout.Bounds = bounds.ConvertToType<Rectangle>();
                        }
                        catch (Exception ex)
                        {
                            This.Logger.Warning(
                                $"Invalid 'bounds' value for key {key} in layout file {layoutSettingsFileName}: {bounds}",
                                ex);
                            formLayout.Bounds = null;
                        }
                    }
                    else
                    {
                        This.Logger.Warning(
                            $"'bounds' value for key {key} in layout file {layoutSettingsFileName} was not found.");
                        formLayout.Bounds = null;
                    }
                }
                else if (xn.Name == "state")
                {
                    var state = GetNodeValue(xn, "value");
                    if (state != null)
                    {
                        try 
                        {
                            formLayout.WindowState = state.ConvertToType<FormWindowState>(); 
                        }
                        catch (Exception ex)
                        {
                            This.Logger.Warning(
                                $"Invalid 'state' value for key {key} in layout file {layoutSettingsFileName}: {state}",
                                ex);
                            formLayout.WindowState = null;
                        }
                    }
                    else
                    {
                        This.Logger.Warning(
                            $"'state' value for key {key} in layout file {layoutSettingsFileName} was not found.");
                        formLayout.WindowState = null;
                    }
                }
                else if (xn.Name == "docking") formLayout.DockingInfo = xn.InnerXml;
                else if (xn.Name == "additionalData" && xn is XmlElement)
                    formLayout.AdditionalData = xn.InnerText;
            }

            layouts.Add(key, formLayout);
        }

        private string GetNodeValue(XmlNode xn, string xaName)
        {
            var xaValue = xn.Attributes
                .Cast<XmlAttribute>().FirstOrDefault(xa => xa.Name == xaName);
            return xaValue?.Value;
        }

        private void SaveLayouts()
        {
            try
            {
                var doc = new XmlDocument();
                _ = doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
                var xnRoot = doc.AppendChild(doc.CreateElement("layouts"));
                foreach (var key in layouts.Keys)
                {
                    var xn = doc.CreateElement("layout");

                    var xa = doc.CreateAttribute("key");
                    xa.Value = key;
                    _ = xn.Attributes.Append(xa);

                    // mandatory, if not present, skip this node
                    var layout = layouts[key];
                    if (layout == null) continue;

                    if (layout.Bounds.HasValue)
                    {
                        var xnBounds = xn.AppendChild(doc.CreateElement("bounds"));
                        var xaBoundsValue = doc.CreateAttribute("value");
                        xaBoundsValue.Value = layout.Bounds.ConvertToString();
                        _ = xnBounds.Attributes.Append(xaBoundsValue);
                    }

                    if (layout.WindowState.HasValue)
                    {
                        var xnState = xn.AppendChild(doc.CreateElement("state"));
                        var xaStateValue = doc.CreateAttribute("value");
                        xaStateValue.Value = layout.WindowState.ConvertToString();
                        _ = xnState.Attributes.Append(xaStateValue);
                    }

                    if (!string.IsNullOrEmpty(layout.DockingInfo))
                    {
                        var xnDocking = xn.AppendChild(doc.CreateElement("docking"));
                        xnDocking.InnerXml = layout.DockingInfo;
                    }

                    if (layout.AdditionalData != null)
                    {
                        var xnAdditionalData = xn.AppendChild(doc.CreateElement("additionalData"));
                        xnAdditionalData.InnerText = layout.AdditionalData;
                    }

                    _ = xnRoot.AppendChild(xn);
                }

                doc.Save(layoutSettingsFileName);
            }
            catch (Exception ex)
            {
                This.Logger.Error(string.Format(
                    "Unable to serialize layout information to file {0}.", layoutSettingsFileName), ex);
            }
        }

        private bool IsLoaded(string key) => loadedForms.Contains(key);

        private void LoadForm(string key)
        {
            if (IsLoaded(key)) return;

            using (LockEvents())
            {
                if (!formsByKey.ContainsKey(key)) return;

                var form = formsByKey[key];

                if (layouts.ContainsKey(key))
                {
                    var layout = layouts[key];
                    if (layout.Bounds.HasValue)
                    {
                        form.StartPosition = FormStartPosition.Manual;
                        var bounds = layout.Bounds.Value;
                        if (!Screen.PrimaryScreen.WorkingArea.Contains(bounds))
                            bounds = DefaultBounds;
                        form.Bounds = bounds;
                    }
                    else This.Logger.Verbose($"Could not find a 'bounds' value for layout key {key}");

                    if (layout.WindowState.HasValue)
                    {
                        var state = layout.WindowState.Value;
                        form.WindowState = state == FormWindowState.Minimized ?
                            FormWindowState.Normal :
                            state;
                    }
                    else
                    {
                        // default to Normal
                        form.WindowState = FormWindowState.Normal;
                        This.Logger.Verbose(string.Format("Could not find a 'state' value for layout key {0}", key));
                    }

                    // If state is Normal and we have no bounds, use DefaultBounds and center on screen
                    if (!layout.Bounds.HasValue && form.WindowState == FormWindowState.Normal)
                    {
                        form.Size = DefaultBounds.Size;
                        form.StartPosition = form.ParentForm == null ?
                            FormStartPosition.CenterScreen :
                            FormStartPosition.CenterParent;
                    }
                }
                else // Creation and initialization
                {
                    var newLayout = new FormLayout();
                    if (form.WindowState != FormWindowState.Normal)
                        newLayout.Bounds = DefaultBounds;
                    layouts.Add(key, newLayout);
                }

                form.SizeChanged += (s, e) => UpdateForm(key);
                form.LocationChanged += (s, e) => UpdateForm(key);
                form.FormClosed += (s, e) => UnloadForm(key);

                loadedForms.Add(key);

                UpdateForm(key);

                // Additional layout data.
                // Even if no additional data was saved, call SetData.
                if (form is IAdditionalLayoutDataSource source)
                    source.SetAdditionalLayoutData(layouts[key].AdditionalData);
            }
        }

        private void UpdateForm(string key)
        {
            if (EventsLocked) return;
            if (!IsLoaded(key)) return;

            using (LockEvents())
            {
                if (!formsByKey.ContainsKey(key)) return;
                var form = formsByKey[key];
                var layout = layouts[key];

                layout.WindowState = form.WindowState;
                layout.Bounds = form.Bounds;
            }
        }

        private void UnloadForm(string key)
        {
            UpdateForm(key);

            // save workspace docking state
            if (layouts.ContainsKey(key))
            {
                var layout = layouts[key];
                if (workspaces.ContainsKey(key))
                {
                    var workspace = workspaces[key];
                    using var stream = new MemoryStream();
                    workspace.SaveAsXml(stream, Encoding.UTF8, true);
                    stream.Flush();
                    _ = stream.Seek(0L, SeekOrigin.Begin);
                    using (var reader = new StreamReader(stream))
                    {
                        layout.DockingInfo = reader.ReadToEnd();
                        reader.Close();
                    }

                    stream.Close();
                }

                var form = formsByKey[key];
                if (form is IAdditionalLayoutDataSource source)
                    layout.AdditionalData = source.GetAdditionalLayoutData();
            }

            SaveLayouts();

            _ = loadedForms.Remove(key);
        }
    }
}

