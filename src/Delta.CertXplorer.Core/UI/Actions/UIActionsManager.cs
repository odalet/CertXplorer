/* 
 * Grabbed from Marco De Sanctis' Actions
 * see http://blogs.ugidotnet.org/crad/articles/38329.aspx
 * Original namespace: Crad.Windows.Forms.Actions
 * License: Common Public License Version 1.0
 * 
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;

namespace Delta.CertXplorer.UI.Actions
{
    [ProvideProperty("Action", typeof(Component))]
    [ToolboxItemFilter("System.Windows.Forms")]
    [ToolboxBitmap(typeof(UIAction), "UIActionsManager.bmp")]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms conventions")]
    public class UIActionsManager : Component, IExtenderProvider, ISupportInitialize
    {
        private readonly Dictionary<Component, UIAction> targets;
        private ContainerControl containerControl = null;
        private bool enabled = true;
        private bool initializing = false;

        public UIActionsManager()
        {
            Actions = new UIActionCollection(this);
            targets = new Dictionary<Component, UIAction>();
            TypeDescriptions = new Dictionary<Type, UIActionTargetDescriptor>();

            if (!DesignMode) Application.Idle += new EventHandler(Application_Idle);
        }

        public event EventHandler Update;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public UIActionCollection Actions { get; }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<Type, UIActionTargetDescriptor> TypeDescriptions { get; }

        [DefaultValue(true)]
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;
                enabled = value;
                RefreshActions();
            }
        }

        public ContainerControl ContainerControl
        {
            get => containerControl;
            set => SetContainerControl(value);
        }

        [Browsable(false)]
        public Control ActiveControl => GetActiveControl(containerControl);

        public override ISite Site
        {
            get => base.Site;
            set
            {
                base.Site = value;
                if (value != null &&
                    value.GetService(typeof(IDesignerHost)) is IDesignerHost host &&
                    host.RootComponent is ContainerControl control)
                    SetContainerControl(control);
            }
        }

        [DefaultValue(null)]
        public UIAction GetAction(Component extendee) => targets.ContainsKey(extendee) ? targets[extendee] : null;

        public void SetAction(Component extendee, UIAction action)
        {
            if (!initializing)
            {
                if (extendee == null) throw new ArgumentNullException(nameof(extendee));
                if (action != null && action.ActionList != this)
                    throw new ArgumentException("The Action you selected is owned by another ActionList", nameof(action));
            }

            if (targets.ContainsKey(extendee))
            {
                targets[extendee].InternalRemoveTarget(extendee);
                _ = targets.Remove(extendee);
            }

            if (action != null)
            {
                if (!TypeDescriptions.ContainsKey(extendee.GetType())) TypeDescriptions.Add(
                    extendee.GetType(), new UIActionTargetDescriptor(extendee.GetType()));

                targets.Add(extendee, action);
                action.InternalAddTarget(extendee);
            }
        }

        protected virtual void OnUpdate(EventArgs eventArgs)
        {
            Update?.Invoke(this, eventArgs);
            foreach (var action in Actions) action.DoUpdate();
        }

        protected virtual Type[] GetSupportedTypes() => new Type[]
        {
            typeof(ButtonBase),
            typeof(ToolStripButton),
            typeof(ToolStripMenuItem),
            typeof(ToolBarButton),
            typeof(MenuItem)
        };

        private void RefreshActions()
        {
            if (DesignMode) return;
            foreach (var action in Actions) action.RefreshEnabledCheckState();
        }

        private void CheckInternalCollections()
        {
            foreach (var action in targets.Values)
            {
                if (!Actions.Contains(action) || action.ActionList != this) throw new InvalidOperationException(
                    "Action owned by another action list or invalid Action.ActionList");
            }
        }

        private Control GetActiveControl(ContainerControl container)
        {
            if (container == null) return null;
            return container.ActiveControl is ContainerControl cc ? GetActiveControl(cc) : container.ActiveControl;
        }

        private void SetContainerControl(ContainerControl container)
        {
            if (containerControl == container) return;

            containerControl = container;
            if (!DesignMode && containerControl is Form f)
            {
                f.KeyPreview = true;
                f.KeyDown += new KeyEventHandler(form_KeyDown);
            }
        }

        private void Application_Idle(object sender, EventArgs e) => OnUpdate(EventArgs.Empty);

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            foreach (var action in Actions)
            {
                if (action.ShortcutKeys == e.KeyData)
                    action.RunShortcut();
            }
        }

        bool IExtenderProvider.CanExtend(object extendee)
        {
            var targetType = extendee.GetType();
            foreach (var type in GetSupportedTypes())
            {
                if (type.IsAssignableFrom(targetType)) return true;
            }

            return false;
        }

        public void BeginInit() => initializing = true;

        public void EndInit()
        {
            initializing = false;
            CheckInternalCollections();
            RefreshActions();
        }
    }
}
