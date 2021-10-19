using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.Plugin.Components;
using OpenTabletDriver.Plugin.Settings;

#nullable enable

namespace OpenTabletDriver
{
    public sealed class PluginContainer : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PluginTypesProvider _pluginTypesProvider;
        private readonly List<object> _activePlugins = new();

        public PluginContainer(IServiceProvider serviceProvider, PluginTypesProvider pluginTypesProvider)
        {
            _serviceProvider = serviceProvider;
            _pluginTypesProvider = pluginTypesProvider;
        }

        public static PluginContainer CreateContainer(IServiceProvider serviceProvider)
        {
            return new PluginContainer(
                serviceProvider,
                serviceProvider.GetRequiredService<PluginTypesProvider>()
            );
        }

        public ImmutableArray<Type> GetImplementingTypes<T>()
        {
            return _pluginTypesProvider.Types
                .Where(static t => t.IsAssignableFrom(typeof(T)))
                .ToImmutableArray();
        }

        public Type GetTypeFromPath(string path)
        {
            return _pluginTypesProvider.Types.Where(t => t.FullName == path).FirstOrDefault()
                ?? throw PluginNotFoundException.Create(path);
        }

        public T CreateInstance<T>(Type pluginType, PluginSettings pluginSettings)
        {
            if (!PluginTypesProvider.ImplementablePluginTypes.Contains(typeof(T)))
                throw new ArgumentException($"'{typeof(T).Name}' is not an implementable plugin type", nameof(pluginType));
            if (!pluginType.IsAssignableTo(typeof(T)))
                throw new ArgumentException($"Plugin '{pluginType.FullName}' is not assignable to '{typeof(T).Name}'", nameof(pluginType));
            if (pluginSettings.PluginTypeName != pluginType.FullName)
                throw new ArgumentException($"Provided plugin settings is not compatible to plugin '{pluginType.FullName}'", nameof(pluginSettings));

            var plugin = ActivatorUtilities.CreateInstance(_serviceProvider, pluginType);
            pluginSettings.ApplyTo(plugin);

            lock (_activePlugins)
            {
                _activePlugins.Add(plugin);
            }

            return (T)plugin;
        }

        public bool ContainsInstanceOf(Type pluginType)
        {
            return _activePlugins.Any(p => p.GetType() == pluginType);
        }

        public void SetPluginSettings(PluginSettings pluginSettings)
        {
            lock (_activePlugins)
            {
                foreach (var plugin in _activePlugins)
                {
                    if (plugin.GetType().FullName == pluginSettings.PluginTypeName)
                        pluginSettings.ApplyTo(plugin);
                }
            }
        }

        public void Dispose()
        {
            foreach (var plugin in _activePlugins.OfType<IDisposable>())
            {
                plugin.Dispose();
            }
        }
    }
}