using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Delta.CertXplorer.Extensibility;

namespace Delta.CertXplorer.PluginsManagement
{
    public sealed class PluginsManager
    {
        // This dictionary stores the plugins unique ids and their short name.
        private readonly Dictionary<Guid, string> pluginNames = new Dictionary<Guid, string>();
        private readonly List<IPlugin> initializedPlugins = new List<IPlugin>();
        private readonly ServiceContainer globalServices = new ServiceContainer();
        private readonly string[] pluginsDirectories;

        public PluginsManager(IEnumerable<string> pluginsDirectoriesArray)
        {
            pluginsDirectories = ParsePluginDirectories(pluginsDirectoriesArray).ToArray();
            globalServices.AddService<IHostService>(new HostService());
        }

        [ImportMany]
        public IEnumerable<IGlobalPlugin> GlobalPlugins { get; set; }

        [ImportMany]
        public IEnumerable<IDataHandlerPlugin> DataHandlerPlugins { get; set; }

        public IEnumerable<IPlugin> Plugins => GlobalPlugins.Cast<IPlugin>().Union(DataHandlerPlugins.Cast<IPlugin>());

        public void Compose()
        {
            GlobalPlugins = new List<IGlobalPlugin>();
            DataHandlerPlugins = new List<IDataHandlerPlugin>();

            try
            {
                var directoryCatalogs = pluginsDirectories.Select(d => new DirectoryCatalog(d));
                var catalog = new AggregateCatalog(directoryCatalogs.ToArray());
                var container = new CompositionContainer(catalog);

                var batch = new CompositionBatch();
                _ = batch.AddPart(this);

                container.Compose(batch);
            }
            catch (Exception ex)
            {
                This.Logger.Error($"Could not load plugins: {ex.Message}", ex);
            }
        }

        public void Run(IGlobalPlugin plugin, IWin32Window parent, out bool shouldDisable)
        {
            shouldDisable = true; // default

            if (plugin == null)
            {
                This.Logger.Error("Null plugins are not allowed");                
                return;
            }

            if (!Initialize(plugin))
                return;

            if (!plugin.Run(parent))
                This.Logger.Warning("This plugin notified that it ended in an error state");

            shouldDisable = false; // This plugin is ok
        }

        public bool Initialize(IPlugin plugin)
        {
            if (initializedPlugins.Contains(plugin))
                return true;

            try 
            {
                CheckPlugin(plugin);

                // add local services
                var services = new ServiceContainer(globalServices);                
                services.AddService<Extensibility.Logging.ILogService>(
                    new PluginsLogService(pluginNames[plugin.PluginInfo.Id]));

                plugin.Initialize(services);

                return true;
            }
            catch (Exception ex)
            {
                This.Logger.Error($"Plugin initialization failed: {ex.Message}", ex);
                return false;
            }
            finally
            {
                initializedPlugins.Add(plugin);
            }            
        }

        private void CheckPlugin(IPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));
            if (plugin.PluginInfo == null) throw new NullReferenceException("The PlugInfo member can't be null");
            if (plugin.PluginInfo.Id == Guid.Empty) throw new NullReferenceException("The PlugInfo.Id member must be set");

            var id = plugin.PluginInfo.Id;
            if (pluginNames.ContainsKey(id)) throw new ApplicationException($"A plugin with id {id} was already loaded");

            // Now build the plugin's short name (it must be unique too).
            var name = plugin.PluginInfo.Name;
            if (string.IsNullOrEmpty(name)) name = id.ToString();

            if (pluginNames.Values.Contains(name))
            {
                var version = plugin.PluginInfo.Version;
                if (!string.IsNullOrEmpty(version))
                    name = $"{name} {version}";
            }

            // this is very unlikely...
            if (pluginNames.Values.Contains(name))
                name = $"{name} [{id}]";

            // most improbable...
            if (pluginNames.Values.Contains(name))
            {
                var index = 1;
                var baseName = name;
                while (pluginNames.Values.Contains(name))
                {
                    name = $"{baseName} ({index})";
                    index++;
                }
            }

            pluginNames.Add(id, name);
        }

        private static IEnumerable<string> ParsePluginDirectories(IEnumerable<string> directories)
        {
            var list = directories == null ? new List<string>(new[] { "." }) : directories.ToList();
            try
            {
                return list
                    .Select(d => Path.IsPathRooted(d) ?
                        d :
                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), d))
                    .Where(d =>
                    {
                        var exists = Directory.Exists(d);
                        if (!exists)
                            This.Logger.Warning(string.Format("Plugins directory {0} doesn't exist.", d));
                        return exists;
                    })
                    .ToArray();
            }
            catch (Exception ex)
            {
                This.Logger.Error("Error while parsing the plugins directories.", ex);
                return new string[] { "." };
            }
        }
    }
}
