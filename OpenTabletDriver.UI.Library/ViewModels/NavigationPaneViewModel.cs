using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.ViewModels;

public partial class NavigationPaneViewModel : ViewModelBase
{
    private readonly IDaemonService _daemonService;
    private readonly INavigator _navigator;

    [ObservableProperty]
    private NavigationItem? _selectedNavigation;

    [ObservableProperty]
    private bool _settingsOpened;

    public ObservableCollection<NavigationItem> Navigations { get; } = new();

    public NavigationPaneViewModel(IDaemonService daemonService, INavigatorFactory navigatorFactory)
    {
        _daemonService = daemonService;
        _navigator = navigatorFactory.GetOrCreate(AppRoutes.MainHost);

        daemonService.HandleProperty(
            nameof(IDaemonService.State),
            d => d.State,
            (d, s) =>
            {
                // clear the navigation list to workaround an avalonia layout bug
                // unfortunately due to this we can't animate the Daemon item into
                // popping up
                Navigations.Clear();

                var daemonDependentsEnabled = s == DaemonState.Connected;
                if (!daemonDependentsEnabled)
                {
                    Navigations.Add(new NavigationItem("Daemon", AppRoutes.DaemonConnectionRoute));
                }

                var tablets = new NavigationItem("Tablets", AppRoutes.TabletsOverviewRoute, daemonDependentsEnabled);
                var tools = new NavigationItem("Tools", AppRoutes.ToolsSettingsRoute, daemonDependentsEnabled);
                var pluginManager = new NavigationItem("Plugin Manager", AppRoutes.PluginManagerRoute, daemonDependentsEnabled);
                var diagnostics = new NavigationItem("Diagnostics", AppRoutes.DiagnosticsRoute);

                Navigations.Add(tablets);
                Navigations.Add(tools);
                Navigations.Add(pluginManager);
                Navigations.Add(diagnostics);
            });
    }

    partial void OnSelectedNavigationChanged(NavigationItem? value)
    {
        if (value != null)
        {
            _navigator.Push(value.Route, asRoot: true);
            SettingsOpened = false;
        }
    }

    partial void OnSettingsOpenedChanged(bool value)
    {
        if (value)
        {
            _navigator.Push(AppRoutes.SettingsRoute, asRoot: true);
            SelectedNavigation = null;
        }
    }
}

public partial class NavigationItem : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _route;

    [ObservableProperty]
    private bool _isEnabled;

    public NavigationItem(string name, string route, bool isEnabled = true)
    {
        _name = name;
        _route = route;
        _isEnabled = isEnabled;
    }
}
