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

    public ObservableCollection<NavigationItem> Navigations { get; } = new()
    {
#if DEBUG
        new NavigationItem("Daemon (DEBUG)", AppRoutes.DaemonConnectionRoute),
#endif
        new NavigationItem("Tablets", AppRoutes.TabletsOverviewRoute),
        new NavigationItem("Tools", AppRoutes.ToolsSettingsRoute),
        new NavigationItem("Plugin Manager", AppRoutes.PluginManagerRoute),
        new NavigationItem("Diagnostics", AppRoutes.DiagnosticsRoute),
    };

    public NavigationPaneViewModel(IDaemonService daemonService, INavigator navigator)
    {
        _daemonService = daemonService;
        _navigator = navigator;

        var tablets = Navigations.First(n => n.Name == "Tablets");
        var tools = Navigations.First(n => n.Name == "Tools");
        var pluginManager = Navigations.First(n => n.Name == "Plugin Manager");

        daemonService.HandleProperty(
            nameof(IDaemonService.State),
            d => d.State,
            (d, s) =>
            {
                tablets.IsEnabled = s == DaemonState.Connected;
                tools.IsEnabled = s == DaemonState.Connected;
                pluginManager.IsEnabled = s == DaemonState.Connected;
            });
    }

    partial void OnSelectedNavigationChanged(NavigationItem? value)
    {
        if (value != null)
        {
            _navigator.NextAsRoot(value.Route);
            SettingsOpened = false;
        }
    }

    partial void OnSettingsOpenedChanged(bool value)
    {
        if (value)
        {
            _navigator.NextAsRoot(AppRoutes.SettingsRoute);
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
