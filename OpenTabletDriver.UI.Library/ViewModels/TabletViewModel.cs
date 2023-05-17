using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.ViewModels;

public class TabletViewModel : ActivatableViewModelBase
{
    private ITabletService _tabletService;

    public int TabletId { get; }
    public string Name { get; set; }

    public TabletViewModel(ITabletService tabletService)
    {
        _tabletService = tabletService;

        Name = tabletService.Name;
    }
}
