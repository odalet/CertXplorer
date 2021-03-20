using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    public partial class PropertyGridEx : PropertyGrid
    {
        private readonly DesignerVerbCollection verbs;
        private PropertyGridExContainer container;
        private object[] wrappedObjects;
        private object selectedObject;
        private object[] selectedObjects;

        public PropertyGridEx() : base()
        {
            verbs = new DesignerVerbCollection();
            ToolStripRenderer = VS2015ThemeProvider.Renderer;
            InitializeComponentModel();            
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripRenderer Renderer
        {
            get => ToolStripRenderer;
            set => ToolStripRenderer = value;
        }

        public new object SelectedObject
        {
            get => selectedObject;
            set => SelectObject(value);
        }

        public new object[] SelectedObjects
        {
            get => selectedObjects;
            set => SelectObjects(value);
        }

        public void AddVerb(DesignerVerb verb)
        {
            if (verb != null) _ = verbs.Add(verb);
        }

        public void RemoveVerb(DesignerVerb verb)
        {
            if (verbs.Contains(verb)) verbs.Remove(verb);
        }

        private void InitializeComponentModel()
        {
            container = new PropertyGridExContainer();
            container.Services.AddService<IMenuCommandService>(new MenuCommandService(verbs));
        }

        private void OnSelectionAboutToChange()
        {
            // We clear the components
            var components = new List<IComponent>(container.Components.Cast<IComponent>());
            components.ForEach(c => container.Remove(c));

            // We add the new selection to the container.
            foreach (var c in wrappedObjects.Cast<IComponent>())
            {
                c.Site = new PropertyGridExContainer.Site(c, null, container);
                container.Add(c);
            }
        }

        private void SelectObject(object selection)
        {
            if (selection == null)
            {
                selectedObject = null;
                base.SelectedObject = null;
                return;
            }

            selectedObject = selection;

            var wrapped = new ObjectWrapper(selection);
            wrappedObjects = new[] { wrapped };
            OnSelectionAboutToChange();
            base.SelectedObject = wrapped;
        }

        private void SelectObjects(object[] selection)
        {
            if (selection == null)
            {
                selectedObjects = null;
                base.SelectedObjects = null;
                return;
            }

            selectedObjects = selection;
            wrappedObjects = selection.Select(o => new ObjectWrapper(o)).ToArray();
            OnSelectionAboutToChange();

            base.SelectedObjects = wrappedObjects;
        }
    }
}
