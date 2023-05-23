using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.ViewModels;

public partial class TabletViewModel : ActivatableViewModelBase
{
    private IDaemonService _daemonService;
    private ITabletService _tabletService;

    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty]
    private AreaDisplayViewModel _displayArea = null!;

    [ObservableProperty]
    private AreaDisplayViewModel _tabletArea = null!;

    public int TabletId { get; }
    public string Name { get; set; }

    public TabletViewModel(ITabletService tabletService)
    {
        _daemonService = Ioc.Default.GetRequiredService<IDaemonService>();
        _tabletService = tabletService;

        TabletId = tabletService.TabletId;
        Name = tabletService.Name;

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        var daemon = _daemonService.Instance!;
        var displayDtos = await daemon.GetDisplays();
        var displayBounds = displayDtos
            .OrderBy(d => d.Index)
            .Select(d => Bounds.FromDto(d));

        // TODO: check settings for display and tablet area

        DisplayArea = new AreaDisplayViewModel(displayBounds, null, "Full Virtual Desktop");

        var tabletWidth = _tabletService.Configuration.Specifications.Digitizer!.MaxX;
        var tabletHeight = _tabletService.Configuration.Specifications.Digitizer!.MaxY;
        var tabletBounds = new Bounds(0, 0, (int)tabletWidth, (int)tabletHeight, 0, "Full Area");

        TabletArea = new AreaDisplayViewModel(new Bounds[] { tabletBounds }, null, "Full Area");

        IsInitialized = true;
    }
}
