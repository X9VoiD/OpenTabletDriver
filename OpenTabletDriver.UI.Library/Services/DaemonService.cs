using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Daemon.Contracts;
using OpenTabletDriver.Daemon.Contracts.RPC;
using OpenTabletDriver.Logging;
using OpenTabletDriver.UI.Models;

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
    ObservableCollection<ITabletService> Tablets { get; }
    Task ConnectAsync();
    Task ConnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    Task ReconnectAsync();
    Task ReconnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
}

public partial class DaemonService : ObservableObject, IDaemonService
{
    private readonly IRpcClient<IDriverDaemon> _rpcClient;
    private readonly IDispatcher _dispatcher;
    private DaemonState _state;
    private IDriverDaemon? _instance;
    private bool _suppressStateChangedEvents;

    public DaemonState State
    {
        get => _state;
        private set => SetProperty(ref _state, value);
    }

    public IDriverDaemon? Instance
    {
        get => _instance;
        private set => SetProperty(ref _instance, value);
    }

    public ObservableCollection<ITabletService> Tablets { get; } = new();

    public DaemonService(IRpcClient<IDriverDaemon> rpcClient, IDispatcher dispatcher)
    {
        _rpcClient = rpcClient;
        _dispatcher = dispatcher;
        rpcClient.Connected += OnConnected;
        rpcClient.Disconnected += OnDisconnected;
    }

    public Task ConnectAsync()
    {
        return ConnectAsync(TimeSpan.FromSeconds(5));
    }

    public async Task ConnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (!_rpcClient.IsConnected)
        {
            State = DaemonState.Connecting;
            try
            {
                await _rpcClient.ConnectAsync(timeout, cancellationToken);
            }
            catch
            {
                State = DaemonState.Disconnected;
                throw;
            }
        }
    }

    public Task ReconnectAsync()
    {
        return ReconnectAsync(TimeSpan.FromSeconds(5));
    }

    public async Task ReconnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        _suppressStateChangedEvents = true;
        State = DaemonState.Connecting;
        _rpcClient.Disconnect();
        await _rpcClient.ConnectAsync(timeout, cancellationToken);
        _suppressStateChangedEvents = false;
    }

    private async void OnConnected(object? _, EventArgs args)
    {
        var daemon = _rpcClient.Instance!;
        Log.Output += Log_Output;

        daemon.TabletAdded += (sender, tabletId) => CreateTabletService(daemon, tabletId);
        daemon.TabletRemoved += (sender, tabletId) => RemoveTabletService(tabletId);
        (await daemon.GetTablets()).ForEach(tabletId => CreateTabletService(daemon, tabletId));

        Instance = daemon;

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

    private void CreateTabletService(IDriverDaemon daemon, int tabletId)
    {
        _dispatcher.Post(async () =>
        {
            var tabletService = await TabletService.CreateAsync(daemon, tabletId);
            Tablets.Add(tabletService);
        });
    }

    private void RemoveTabletService(int tabletId)
    {
        _dispatcher.ProbablySynchronousPost(() =>
        {
            var tabletService = Tablets.FirstOrDefault(tablet => tablet.TabletId == tabletId);
            if (tabletService is not null)
                Tablets.Remove(tabletService);
        });
    }
}
