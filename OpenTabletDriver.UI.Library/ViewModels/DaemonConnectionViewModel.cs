using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.UI.Navigation;
using OpenTabletDriver.UI.Services;

namespace OpenTabletDriver.UI.ViewModels;

public partial class DaemonConnectionViewModel : NavigationViewModelBase
{
    private readonly IDaemonService _daemonService;
    private int _retryCount = 0;

    [ObservableProperty]
    private string _mainText;

    [ObservableProperty]
    private List<string> _qolHintText = new();

    [ObservableProperty]
    private bool _showButtons;

    [ObservableProperty]
    private bool _showQolHintText;

    public DaemonConnectionViewModel(IDaemonService daemonService)
    {
        _daemonService = daemonService;
        _daemonService.PropertyChanged += DaemonService_OnPropertyChanged;
        _mainText = GetMainText(_daemonService.State);

        // TODO: Change QoL hint according to environment
        _qolHintText.Add("""
        Please make sure that OpenTabletDriver.Daemon is running or is in the same directory as OpenTabletDriver.UX.
        """);
        _qolHintText.Add("""
        Click the "Help" button for more information.
        """);
    }

    private void DaemonService_OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(IDaemonService.State))
        {
            var state = _daemonService.State;
            MainText = GetMainText(state);

            if (state == DaemonState.Connected)
            {
                _retryCount = 0;
                ShowQolHintText = false;
            }

            // Show hint on second failed connection attempt
            if (state == DaemonState.Disconnected && _retryCount > 0)
            {
                ShowQolHintText = true;
            }

            ShowButtons = state == DaemonState.Disconnected;
        }
    }

    private static string GetMainText(DaemonState state)
    {
        return state switch
        {
            DaemonState.Disconnected => "Daemon is not running! ~(>_<~)",
            DaemonState.Connecting => "Connecting to daemon... |･ω･)",
            DaemonState.Connected => "Connected to daemon. (◕‿◕✿)",
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    [RelayCommand]
    public async Task ConnectAsync()
    {
        try
        {
            _retryCount++;
            await _daemonService.ConnectAsync();
        }
        catch
        {
            //
        }
    }

    [RelayCommand]
    public static void GoToHelpWebsite()
    {
        // TODO: link to a more specific wiki page
        IoHelpers.OpenLink("https://opentabletdriver.net/Wiki");
    }

    public override string PageName => "Placeholder";
}
