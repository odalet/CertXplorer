using System;
using System.Collections.Generic;
using SCD = System.ComponentModel.Design;
using Delta.CertXplorer.Services;

namespace Delta.CertXplorer.CertManager
{
    /// <summary>
    /// Global selection service.
    /// </summary>
    internal class GlobalSelectionService : ISelectionService
    {
        private readonly Dictionary<ISelectionSource, EventHandler> sources = new Dictionary<ISelectionSource, EventHandler>();

        /// <summary>
        /// Gets or create the global selection service.
        /// </summary>
        /// <param name="serviceContainer">The service container in which an instance of the selection service will be added (or retrieved).</param>
        /// <returns>An instance of a class implemenaing <see cref="Delta.CertXplorer.Common.Services.ISelectionService"/>.</returns>
        public static ISelectionService GetOrCreateSelectionService(SCD.IServiceContainer serviceContainer)
        {
            if (serviceContainer == null) throw new ArgumentNullException("serviceContainer");

            var selectionService = serviceContainer.GetService<ISelectionService>();
            if (selectionService == null)
            {
                selectionService = new GlobalSelectionService();
                serviceContainer.AddService<ISelectionService>(selectionService);
            }

            return selectionService;
        }

        public event EventHandler SelectionChanged;

        public object SelectedObject { get; protected set; }
        public object CurrentSource { get; private set; }

        public void AddSource(ISelectionSource selectionSource)
        {
            if (sources.ContainsKey(selectionSource)) return;

            void handler(object s, EventArgs e)
            {
                if (s is ISelectionSource source) OnSelectionChanged(source, source.SelectedObject);
            }

            selectionSource.SelectionChanged += handler;
            sources.Add(selectionSource, handler);
        }

        public void RemoveSource(ISelectionSource selectionSource)
        {
            if (!sources.ContainsKey(selectionSource)) return;

            var handler = sources[selectionSource];
            selectionSource.SelectionChanged -= handler;
            _ = sources.Remove(selectionSource);
        }

        protected virtual void OnSelectionChanged(object source, object selectedObject)
        {
            CurrentSource = source;
            SelectedObject = selectedObject;
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
