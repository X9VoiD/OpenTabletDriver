using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.Daemon.Contracts.RPC;
using OpenTabletDriver.Logging;

namespace OpenTabletDriver.UI.Services;

public enum DaemonState
{
    Disconnected,
    Connecting,
    Connected
}

public interface IDaemonService : INotifyPropertyChanged, INotifyPropertyChanging
{
    DaemonState State { get; }
    IDriverDaemon? Instance { get; }
    Task ConnectAsync();
    Task ReconnectAsync();
}

public partial class DaemonService : ObservableObject, IDaemonService
{
    private readonly IRpcClient<IDriverDaemon> _rpcClient;
    private IDriverDaemon? _instance;
    private bool _suppressStateChangedEvents;

    [ObservableProperty]
    private DaemonState _state;

    public IDriverDaemon? Instance
    {
        get => _instance;
        private set => SetProperty(ref _instance, value);
    }

    public DaemonService(IRpcClient<IDriverDaemon> rpcClient)
    {
        _rpcClient = rpcClient;
        rpcClient.Connected += OnConnected;
        rpcClient.Disconnected += OnDisconnected;
    }

    public async Task ConnectAsync()
    {
        if (!_rpcClient.IsConnected)
        {
            State = DaemonState.Connecting;
            try
            {
                await _rpcClient.ConnectAsync();
            }
            catch
            {
                State = DaemonState.Disconnected;
                throw;
            }
        }
    }

    public async Task ReconnectAsync()
    {
        _suppressStateChangedEvents = true;
        State = DaemonState.Connecting;
        _rpcClient.Disconnect();
        await _rpcClient.ConnectAsync();
        _suppressStateChangedEvents = false;
    }

    private void OnConnected(object? _, EventArgs args)
    {
        Instance = _rpcClient.Instance;
        Log.Output += Log_Output;

        if (!_suppressStateChangedEvents)
            State = DaemonState.Connected;
    }

    private void OnDisconnected(object? _, EventArgs args)
    {
        Instance = null;
        Log.Output -= Log_Output;

        if (!_suppressStateChangedEvents)
            State = DaemonState.Disconnected;
    }

    private void Log_Output(object? _, LogMessage message)
    {
        _instance!.WriteMessage(message).ConfigureAwait(false);
    }
}
