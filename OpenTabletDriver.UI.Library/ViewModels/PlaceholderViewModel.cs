using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.Daemon.Contracts.RPC;
using OpenTabletDriver.UI.Navigation;

namespace OpenTabletDriver.UI.ViewModels;

public partial class PlaceholderViewModel : NavigationViewModelBase
{
    private readonly IRpcClient<IDriverDaemon> _rpc;

    public PlaceholderViewModel(IRpcClient<IDriverDaemon> rpc)
    {
        _rpc = rpc;
    }

    [RelayCommand]
    public async Task ConnectAsync()
    {
        await _rpc.ConnectAsync();
    }

    [RelayCommand]
    public static void GoToHelpWebsite()
    {
        IoHelpers.OpenLink("https://opentabletdriver.net/Wiki");
    }

    public override string PageName => "Placeholder";
}
