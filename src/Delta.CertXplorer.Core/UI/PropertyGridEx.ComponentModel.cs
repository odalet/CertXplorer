using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Delta.CertXplorer.UI
{
    partial class PropertyGridEx
    {
        /// <summary>
        /// Partial implementation (only the verbs are available) of
        /// <see cref="System.ComponentModel.Design.IMenuCommandService"/>.
        /// </summary>
        private sealed class MenuCommandService : IMenuCommandService
        {
            public MenuCommandService() : this(null) { }
            public MenuCommandService(DesignerVerbCollection verbs) => Verbs = verbs;

            public DesignerVerbCollection Verbs { get; }

            public void AddCommand(MenuCommand command) => throw new NotSupportedException();
            public MenuCommand FindCommand(CommandID commandID) => throw new NotSupportedException();
            public bool GlobalInvoke(CommandID commandID) => throw new NotSupportedException();
            public void RemoveCommand(MenuCommand command) => throw new NotSupportedException();
            public void ShowContextMenu(CommandID menuID, int x, int y) => throw new NotSupportedException();

            public void AddVerb(DesignerVerb verb)
            {
                if (Verbs != null && verb != null) _ = Verbs.Add(verb);
            }

            public void RemoveVerb(DesignerVerb verb)
            {
                if (Verbs != null && Verbs.Contains(verb))
                    Verbs.Remove(verb);
            }
        }

        private sealed class PropertyGridExContainer : Container
        {
            public sealed class Site : ISite
            {
                private readonly PropertyGridExContainer gridContainer;

                public Site(IComponent component, string name, PropertyGridExContainer container)
                {
                    Component = component ?? throw new ArgumentNullException(nameof(component));
                    gridContainer = container ?? throw new ArgumentNullException(nameof(container));
                    Name = name;
                }

                public IComponent Component { get; }
                public IContainer Container => gridContainer;
                public bool DesignMode => false;
                public string Name { get; set; }

                public object GetService(Type serviceType) => gridContainer.GetService(serviceType);
            }

            public PropertyGridExContainer() : base() => Services = new ServiceContainer();

            public IServiceContainer Services { get; }

            protected override object GetService(Type service)
            {
                var serviceInstance = Services.GetService(service);
                return serviceInstance ?? base.GetService(service);
            }

            protected override ISite CreateSite(IComponent component, string name) => new Site(component, name, this);
        }
    }
}
