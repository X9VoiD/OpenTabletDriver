using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.ViewModels;

public partial class TabletsOverviewViewModel : ActivatableViewModelBase
{
    private readonly IDaemonService _daemonService;

    [ObservableProperty]
    private TabletViewModel? _selectedTablet;

    public ObservableCollection<TabletViewModel> Tablets { get; } = new();

    public TabletsOverviewViewModel(IDaemonService daemonService)
    {
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

    // TODO: navigate to last selected tablet
}
