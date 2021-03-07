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

        private Rectangle defaultBounds = Rectangle.Empty;
        private readonly string layoutSettingsFileName;
        private readonly List<string> loadedForms = new List<string>();
        private readonly Dictionary<string, Form> formsByKey = new Dictionary<string, Form>();
        private readonly Dictionary<string, FormLayout> layouts = new Dictionary<string, FormLayout>();
        private readonly Dictionary<string, DockPanel> workspaces = new Dictionary<string, DockPanel>();
        private readonly Dictionary<string, Dictionary<Guid, ToolWindow>> toolWindows =
            new Dictionary<string, Dictionary<Guid, ToolWindow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutService"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file containing the serialized layout information.</param>
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

        private bool EventsLocked { get; set; } = false;

        private IDisposable LockEvents() => new EventsLock(this);

        /// <inheritdoc/>
        public void RegisterForm(string key, Form form, DockPanel workspace)
        {
            if (formsByKey.ContainsKey(key)) formsByKey.Remove(key);
            formsByKey.Add(key, form);
            LoadForm(key);
            form.Load += (s, e) => LoadForm(key);

            if (workspace != null)
            {
                if (workspaces.ContainsKey(key)) workspaces.Remove(key);
                workspaces.Add(key, workspace);
            }
        }

        /// <inheritdoc/>
        public void RegisterToolWindow(string key, ToolWindow window)
        {
            if (window == null) throw new ArgumentNullException("window");
            if (!workspaces.ContainsKey(key)) throw new ApplicationException(
                "You can't register a tool window, if a workspace was not registered first.");

            Dictionary<Guid, ToolWindow> subDictionary = null;
            if (toolWindows.ContainsKey(key))
            {
                subDictionary = toolWindows[key];
                if (subDictionary.ContainsKey(window.Guid)) throw new ApplicationException(string.Format(
                    "Tool window with Guid {0} was already registered.", window.Guid));
            }
            else
            {
                subDictionary = new Dictionary<Guid, ToolWindow>();
                toolWindows.Add(key, subDictionary);
            }

            subDictionary.Add(window.Guid, window);
        }

        /// <inheritdoc/>
        public bool RestoreDockingState(string key)
        {
            if (!layouts.ContainsKey(key)) return false;
            if (!workspaces.ContainsKey(key)) return false;

            var dockingXml = layouts[key].DockingInfo;
            try
            {
                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(dockingXml);
                    writer.Flush();
                    stream.Seek(0L, SeekOrigin.Begin);
                    workspaces[key].LoadFromXml(stream, target =>
                    {
                        Delta.CertXplorer.Logging.ILogService log = This.Logger;

                        if (string.IsNullOrEmpty(target))
                        {
                            log.Info("Unable to load window: no guid was provided");
                            return null;
                        }

                        Guid guid = Guid.Empty;
                        try { guid = new Guid(target); }
                        catch (Exception ex)
                        {
                            var debugException = ex;
                        }

                        if (guid == Guid.Empty)
                        {
                            log.Info(string.Format("Unable to load window with guid {0}.", target));
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
                            log.Info(string.Format("Unable to load window with guid {0}.", target));
                            return null;
                        }
                        else return window;

                    }, false);

                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                This.Logger.Error(string.Format(
                    "Unable to restore docking state from the layout key {0}.", key), ex);
                return false;
            }

            return true;
        }

        private void ReadLayouts()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(layoutSettingsFileName);
                foreach (XmlNode xn in doc.ChildNodes)
                {
                    if (xn.Name == "layouts")
                    {
                        foreach (XmlNode xnRoot in xn.ChildNodes)
                        {
                            if (xnRoot.Name == "layout")
                            {
                                string key = GetNodeValue(xnRoot, "key");
                                if (key != null)
                                {
                                    try { ReadLayout(xnRoot, key); }
                                    catch (Exception ex)
                                    {
                                        This.Logger.Error(string.Format(
                                            "Unable to deserialize layout information from file {0} for key {1}.",
                                            layoutSettingsFileName, key), ex);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                This.Logger.Error(string.Format(
                    "Unable to deserialize layout information from file {0}.", layoutSettingsFileName), ex);
            }
        }

        private void ReadLayout(XmlNode xnRoot, string key)
        {
            if (layouts.ContainsKey(key)) return;

            var fl = new FormLayout();
            foreach (XmlNode xn in xnRoot.ChildNodes)
            {
                if (xn.Name == "bounds")
                {
                    string bounds = GetNodeValue(xn, "value");
                    if (bounds != null)
                    {
                        try { fl.Bounds = bounds.ConvertToType<Rectangle>(); }
                        catch (Exception ex)
                        {
                            This.Logger.Warning(string.Format(
                                "Invalid 'bounds' value for key {0} in layout file {1}: {2}",
                                key, layoutSettingsFileName, bounds), ex);
                            fl.Bounds = null;
                        }
                    }
                    else
                    {
                        This.Logger.Warning(string.Format(
                            "'bounds' value for key {0} in layout file {1} was not found.",
                            key, layoutSettingsFileName));
                        fl.Bounds = null;
                    }
                }
                else if (xn.Name == "state")
                {
                    string state = GetNodeValue(xn, "value");
                    if (state != null)
                    {
                        try { fl.WindowState = state.ConvertToType<FormWindowState>(); }
                        catch (Exception ex)
                        {
                            This.Logger.Warning(string.Format(
                                "Invalid 'state' value for key {0} in layout file {1}: {2}",
                                key, layoutSettingsFileName, state), ex);
                            fl.WindowState = null;
                        }
                    }
                    else
                    {
                        This.Logger.Warning(string.Format(
                            "'state' value for key {0} in layout file {1} was not found.",
                            key, layoutSettingsFileName));
                        fl.WindowState = null;
                    }
                }
                else if (xn.Name == "docking") fl.DockingInfo = xn.InnerXml;
                else if ((xn.Name == "additionalData") && (xn is XmlElement))
                    fl.AdditionalData = xn.InnerText;
            }

            layouts.Add(key, fl);
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
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
                var xnRoot = doc.AppendChild(doc.CreateElement("layouts"));
                foreach (string key in layouts.Keys)
                {
                    var xn = doc.CreateElement("layout");

                    var xa = doc.CreateAttribute("key");
                    xa.Value = key;
                    xn.Attributes.Append(xa);

                    // mandatory, if not present, skip this node
                    var layout = layouts[key];
                    if (layout == null) continue;

                    if (layout.Bounds.HasValue)
                    {
                        var xnBounds = xn.AppendChild(doc.CreateElement("bounds"));
                        var xaBoundsValue = doc.CreateAttribute("value");
                        xaBoundsValue.Value = layout.Bounds.ConvertToString();
                        xnBounds.Attributes.Append(xaBoundsValue);
                    }

                    if (layout.WindowState.HasValue)
                    {
                        var xnState = xn.AppendChild(doc.CreateElement("state"));
                        var xaStateValue = doc.CreateAttribute("value");
                        xaStateValue.Value = layout.WindowState.ConvertToString();
                        xnState.Attributes.Append(xaStateValue);
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

                    xnRoot.AppendChild(xn);
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

