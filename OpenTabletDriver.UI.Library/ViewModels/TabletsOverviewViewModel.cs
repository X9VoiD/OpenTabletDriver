using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.Services;
using OpenTabletDriver.UI.Views;

namespace OpenTabletDriver.UI.ViewModels;

public partial class TabletsOverviewViewModel : ActivatableViewModelBase
{
    private readonly IDaemonService _daemonService;
    private readonly INavigator _navigator;

    private TabletViewModel? _lastSelectedTablet;

    [ObservableProperty]
    private TabletViewModel? _selectedTablet;

    public ObservableCollection<TabletViewModel> Tablets { get; } = new();

    public TabletsOverviewViewModel(IDaemonService daemonService, INavigatorFactory navigatorFactory)
    {
        _navigator = navigatorFactory.GetOrCreate(AppRoutes.MainHost);

        // Intercept navigation to TabletsOverview and redirect to last selected tablet
        _navigator.Navigating += (_, ev) =>
        {
            if (ev.Kind != NavigationKind.Pop && ev.Current?.GetType() == typeof(TabletsOverview)
                && _lastSelectedTablet != null)
            {
                // TODO: improve cancel API
                ev.Cancel = NavigationCancellationKind.Redirect;
                _navigator.Push(_lastSelectedTablet);
            }
        };

        _daemonService = daemonService;
        _daemonService.Tablets.CollectionChanged += (sender, e) =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems!.Cast<ITabletService>().ForEach(AddTablet);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems!.Cast<ITabletService>().ForEach(RemoveTablet);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Tablets.Clear();
                    _daemonService.Tablets.ForEach(AddTablet);
                    break;
            }
        };

        _daemonService.Tablets.ForEach(AddTablet);
    }

    private void AddTablet(ITabletService tablet)
    {
        var tabletViewModel = new TabletViewModel(tablet);
        Tablets.Add(tabletViewModel);
    }

    private void RemoveTablet(ITabletService tablet)
    {
        var tabletViewModel = Tablets.First(t => t.TabletId == tablet.TabletId);
        Tablets.Remove(tabletViewModel);
    }

    public override void OnDeactivated()
    {
        _lastSelectedTablet = SelectedTablet;
        SelectedTablet = null;
        base.OnDeactivated();
    }

    partial void OnSelectedTabletChanged(TabletViewModel? value)
    {
        if (value is not null)
        {
            _navigator.Push(value);
        }
    }
}
