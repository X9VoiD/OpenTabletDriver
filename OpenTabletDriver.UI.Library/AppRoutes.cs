using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.ViewModels;
using OpenTabletDriver.UI.Views;

namespace OpenTabletDriver.UI;

public static class AppRoutes
{
    // Navigation hosts
    public const string MainHost = "MainHost";

    // Routes
    public const string DaemonConnectionRoute = "DaemonConnection";
    public const string TabletsOverviewRoute = "TabletsOverview";
    public const string TabletMainSettingsRoute = "TabletMainSettings";
    public const string TabletBindingsRoute = "TabletBindings";
    public const string TabletFiltersRoute = "TabletFilters";
    public const string ToolsSettingsRoute = "ToolsSettings";
    public const string PresetsManagerRoute = "PresetsManager";
    public const string PluginManagerRoute = "PluginManager";
    public const string DiagnosticsRoute = "Diagnostics";
    public const string SettingsRoute = "Settings";

    public static IServiceCollection AddGlobalApplicationViewModels(this IServiceCollection services)
    {
        return services
            .AddTransient<MainWindowViewModel>()
            .AddTransient<NavigationPaneViewModel>();
    }

    public static IServiceCollection AddApplicationRoutes(this IServiceCollection services)
    {
        return services
            .AddTransientRoute<Navigation404ViewModel, Navigation404View>("404")
            .AddSingletonRoute<DaemonConnectionViewModel, DaemonConnectionView>(DaemonConnectionRoute)
            .AddSingletonRoute<UISettingsViewModel, UISettingsView>(SettingsRoute) // register as singleton to preserve "modified" state
            .AddTransientRoute<TabletsOverviewViewModel, TabletsOverview>(TabletsOverviewRoute);
        // .AddNavigationRoute<TabletMainSettingsViewModel, TabletMainSettingsView>(TabletMainSettingsRoute)
        // .AddNavigationRoute<TabletBindingsViewModel, TabletBindingsView>(TabletBindingsRoute)
        // .AddNavigationRoute<TabletFiltersViewModel, TabletFiltersView>(TabletFiltersRoute)
        // .AddNavigationRoute<PluginsSettingsViewModel, PluginsSettingsView>(PluginsSettingsRoute)
        // .AddNavigationRoute<PluginManagerViewModel, PluginManagerView>(PluginManagerRoute)
        // .AddNavigationRoute<DiagnosticsViewModel, DiagnosticsView>(DiagnosticsRoute)
    }
}
