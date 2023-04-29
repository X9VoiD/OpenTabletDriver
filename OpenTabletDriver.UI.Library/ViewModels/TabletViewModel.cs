using OpenTabletDriver.UI.Models;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.ViewModels;

public class TabletViewModel : NavigationViewModelBase
{
    private ITabletService _tabletService;

    public int TabletId { get; }
    public override string PageName { get; }
    public string Name { get; set; }

    public TabletViewModel(ITabletService tabletService)
    {
        _tabletService = tabletService;

        PageName = tabletService.Name;
        Name = tabletService.Name;
    }
}
