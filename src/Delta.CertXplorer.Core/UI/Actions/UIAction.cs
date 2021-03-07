/* 
 * Grabbed from Marco De Sanctis' Actions
 * see http://blogs.ugidotnet.org/crad/articles/38329.aspx
 * Original namespace: Crad.Windows.Forms.Actions
 * License: Common Public License Version 1.0
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI.Actions
{
    [ToolboxBitmap(typeof(UIAction), "UIAction.bmp"), DefaultEvent("Run"), StandardAction]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Winforms conventions")]
    public class UIAction : Component
    {
        protected enum ActionWorkingState
        {
            Listening,
            Driving
        }

        private readonly List<Component> targets = new List<Component>();
        private readonly EventHandler clickEventHandler;
        private readonly EventHandler checkStateChangedEventHandler;
        private UIActionsManager actionList = null;
        private CheckState checkState = CheckState.Unchecked;
        private bool enabled = true;
        private bool checkOnClick = false;
        private Keys shortcutKeys = Keys.None;
        private bool visible = true;

        public UIAction()
        {
            WorkingState = ActionWorkingState.Listening;
            clickEventHandler = new EventHandler(target_Click);
            checkStateChangedEventHandler = new EventHandler(target_CheckStateChanged);
        }

        public event EventHandler CheckStateChanged;
        public event CancelEventHandler BeforeRun;
        public event EventHandler Run;
        public event EventHandler AfterRun;
        public event EventHandler Update;

        [DefaultValue(false)]
        public bool Checked
        {
            get => checkState != CheckState.Unchecked;
            set => CheckState = value ? CheckState.Checked : CheckState.Unchecked;
        }

        [DefaultValue(CheckState.Unchecked), UpdatableProperty]
        public CheckState CheckState
        {
            get => checkState;
            set
            {
                if (checkState == value) return;

                checkState = value;
                UpdateAllTargets(nameof(CheckState), value);
                CheckStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DefaultValue(true), UpdatableProperty]
        public bool Enabled
        {
            get => ActionList != null ? enabled && ActionList.Enabled : enabled;
            set
            {
                if (enabled == value) return;
                enabled = value;
                UpdateAllTargets(nameof(Enabled), value);
            }
        }

        [DefaultValue(false), UpdatableProperty]
        public bool CheckOnClick
        {
            get => checkOnClick;
            set
            {
                if (checkOnClick == value) return;
                checkOnClick = value;
                UpdateAllTargets(nameof(CheckOnClick), value);
            }
        }

        [DefaultValue(Keys.None), UpdatableProperty, Localizable(true)]
        public Keys ShortcutKeys
        {
            get => shortcutKeys;
            set
            {
                if (shortcutKeys == value) return;
                shortcutKeys = value;
                var converter = new KeysConverter();
                var text = (string)converter.ConvertTo(value, typeof(string));
                UpdateAllTargets(nameof(ToolStripMenuItem.ShortcutKeyDisplayString), text);
            }
        }

        [DefaultValue(true), UpdatableProperty]
        public bool Visible
        {
            get => visible;
            set
            {
                if (visible == value) return;
                visible = value;
                UpdateAllTargets(nameof(Visible), value);
            }
        }

        protected ActionWorkingState WorkingState { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        protected internal UIActionsManager ActionList
        {
            get => actionList;
            set
            {
                if (actionList != value)
                    actionList = value;
            }
        }

        public void DoRun()
        {
            if (!enabled) return;

            CancelEventArgs e = new CancelEventArgs();
            OnBeforeRun(e);
            if (e.Cancel) return;

            OnRun(EventArgs.Empty);
            OnAfterRun(EventArgs.Empty);
        }

        protected virtual void OnBeforeRun(CancelEventArgs e) => BeforeRun?.Invoke(this, e);
        protected virtual void OnRun(EventArgs e) => Run?.Invoke(this, e);
        protected virtual void OnAfterRun(EventArgs e) => AfterRun?.Invoke(this, e);
        protected virtual void OnUpdate(EventArgs e) => Update?.Invoke(this, e);
        protected virtual void OnRemovingTarget(Component extendee) { }
        protected virtual void OnAddingTarget(Component extendee) { }

        protected virtual void AddHandler(Component extendee)
        {
            var clickEvent = extendee.GetType().GetEvent("Click");
            if (clickEvent != null)
                clickEvent.AddEventHandler(extendee, clickEventHandler);

            var checkStateChangedEvent = extendee.GetType().GetEvent("CheckStateChanged");
            if (checkStateChangedEvent != null)
                checkStateChangedEvent.AddEventHandler(extendee, checkStateChangedEventHandler);

            if (extendee is ToolBarButton button)
                button.Parent.ButtonClick += new ToolBarButtonClickEventHandler(toolbar_ButtonClick);
        }

        protected virtual void RemoveHandler(Component extendee)
        {
            var clickEvent = extendee.GetType().GetEvent("Click");
            if (clickEvent != null)
                clickEvent.RemoveEventHandler(extendee, clickEventHandler);

            var checkStateChangedEvent = extendee.GetType().GetEvent("CheckStateChanged");
            if (checkStateChangedEvent != null)
                checkStateChangedEvent.RemoveEventHandler(extendee, checkStateChangedEventHandler);

            if (extendee is ToolBarButton button)
                button.Parent.ButtonClick -= new ToolBarButtonClickEventHandler(toolbar_ButtonClick);
        }

        internal void DoUpdate() => OnUpdate(EventArgs.Empty);

        internal void InternalRemoveTarget(Component extendee)
        {
            _ = targets.Remove(extendee);
            RemoveHandler(extendee);
            OnRemovingTarget(extendee);
        }

        internal void InternalAddTarget(Component extendee)
        {
            targets.Add(extendee);
            RefreshState(extendee);
            AddHandler(extendee);
            OnAddingTarget(extendee);
        }

        internal void RefreshEnabledCheckState()
        {
            UpdateAllTargets(nameof(Enabled), Enabled);
            UpdateAllTargets(nameof(CheckState), CheckState);
        }

        internal void RunShortcut()
        {
            if (!Enabled) return;
            if (CheckOnClick) Checked = !Checked;
            DoRun();
        }

        protected void UpdateAllTargets(string propertyName, object value)
        {
            foreach (var component in targets)
                UpdateProperty(component, propertyName, value);
        }

        private void UpdateProperty(Component target, string propertyName, object value)
        {
            WorkingState = ActionWorkingState.Driving;
            try
            {
                if (ActionList != null)
                    ActionList.TypeDescriptions[target.GetType()].SetValue(propertyName, target, value);
            }
            finally
            {
                WorkingState = ActionWorkingState.Listening;
            }
        }

        private void RefreshState(Component target)
        {
            var properties = TypeDescriptor.GetProperties(this, new[] { new UpdatablePropertyAttribute() });
            foreach (PropertyDescriptor property in properties)
                UpdateProperty(target, property.Name, property.GetValue(this));
        }

        private void HandleClick(object sender, EventArgs e)
        {
            if (WorkingState != ActionWorkingState.Listening) return;
            DoRun();
        }

        private void HandleCheckStateChanged(object sender, EventArgs e)
        {
            if (WorkingState != ActionWorkingState.Listening) return;
            CheckState = (CheckState)ActionList.TypeDescriptions[sender.GetType()].GetValue("CheckState", sender);
        }

        private void toolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (targets.Contains(e.Button)) HandleClick(e.Button, e); // called if sender is ToolBarButton
        }

        private void target_Click(object sender, EventArgs e) => HandleClick(sender, e); // called if sender is Control

        private void target_CheckStateChanged(object sender, EventArgs e) => HandleCheckStateChanged(sender, e);
    }
}
