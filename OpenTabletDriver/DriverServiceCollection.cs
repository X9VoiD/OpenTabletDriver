using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTabletDriver.ComponentProviders;
using OpenTabletDriver.Configurations;
using OpenTabletDriver.Devices;
using OpenTabletDriver.Plugin.Components;

#nullable enable

namespace OpenTabletDriver
{
    public abstract class DriverServiceCollection : ServiceCollection
    {
        private static IEnumerable<ServiceDescriptor> RequiredServices => new ServiceDescriptor[]
        {
            ServiceDescriptor.Singleton<IReportParserProvider, ReportParserProvider>(),
            ServiceDescriptor.Singleton<IDeviceHubsProvider, DeviceHubsProvider>(serviceProvider => new DeviceHubsProvider(serviceProvider)),
            ServiceDescriptor.Singleton<ICompositeDeviceHub, RootHub>(serviceProvider => RootHub.WithProvider(serviceProvider)),
            ServiceDescriptor.Singleton<IDeviceConfigurationProvider, DeviceConfigurationProvider>(),
            ServiceDescriptor.Singleton<PluginTypesProvider, DefaultPluginTypesProvider>(),
            ServiceDescriptor.Singleton(typeof(TabletSettingsProvider)),
            ServiceDescriptor.Scoped<PluginContainer, PluginContainer>(p => PluginContainer.CreateContainer(p))
        };

        public DriverServiceCollection(DriverDefaults defaultTypes)
        {
            foreach (var serviceDescriptor in RequiredServices)
                this.Add(serviceDescriptor);

            this.AddSingleton(defaultTypes);
        }
    }
}